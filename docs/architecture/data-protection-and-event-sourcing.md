# Data Protection & Event Sourcing — Analysis and Draft Plan

**Date**: 2026-07-21
**Status**: Draft proposal (not yet implemented)
**Scope**: `FinanceBackEnd` domain and persistence layers — how financial/PII data is currently protected, and whether/how an event-sourced change history should be introduced.

This document builds on the existing point-in-time findings in `docs/security-audits/security-audit-backend-domain-2026-03-29.md`, `security-audit-backend-persistence-2026-03-29.md`, and `security-audit-backend-authentication-2026-03-29.md`, and extends them into a forward-looking plan. It does not repeat every finding from those audits verbatim — see them for full detail — but references the ones that are load-bearing for the decisions below.

---

## 1. What data actually lives here

Finanzas is a personal-finance tracker, not a payment processor. A full inventory of `Finance.Domain/Models/` shows the sensitive surface is narrower than "financial app" might suggest:

| Category | Fields | Where |
|---|---|---|
| **PII** | `Username`, `FirstName`, `LastName` | `User` |
| **Identity anchor** | `SourceId` (Auth0 `sub`, e.g. `auth0\|...`) | `Identity` |
| **Financial amounts/balances** | `Amount`, `Total`, `Price`, `BuyRate`/`SellRate`, `MinimumDue`, `UnappliedCredit`, `LastPrice`, `AverageBuyPrice`, `Valued` | `Movement`, `Fund`, `Income`, `Debit`, `CreditCardTransaction`, `CreditCardStatement`, `CreditCardPayment`, `CreditCardStatementAdjustment`, `Subscription`, `CurrencyExchangeRate`, `IOLInvestment` |
| **Ownership graph** | `ResourceId`, `UserId`, `PermissionLevels` | `ResourcePermissions<T>` and its 10 concrete subclasses |

Notably **absent** from the domain model: no card numbers/PAN, no CVV/expiry, no bank account numbers, no passwords, no locally-issued tokens. Authentication is fully delegated to Auth0 (`Finance.Authentication`), and `CreditCard` is a logical ledger account (name + bank + issuer), not a stored payment instrument. This materially narrows the compliance surface — there is no PCI-DSS cardholder data to protect, and the only regulated category present is ordinary account PII plus personal financial transaction history (which is still sensitive and worth protecting deliberately, just not in PCI/PAN terms).

This reframes the two questions in this document:

1. **Data protection** is really about (a) protecting three plaintext name fields and one third-party identity token, (b) making sure the *access control* around financial amounts is airtight (since the amounts themselves are not going to be field-encrypted), and (c) closing integrity gaps that let bad values into the ledger.
2. **Event sourcing** is not about needing an immutable ledger for regulatory reasons — it's about whether the app benefits from a real change history (currently there is none at all beyond `CreatedAt`/`UpdatedAt`) for auditability, "what did my balance look like on date X," and recovering from bad imports/bugs.

---

## 2. Current state of data protection

### 2.1 What protection exists today

- **Transport**: Auth0-issued JWTs validated on every request (issuer/audience/lifetime/JWKS) — `Finance.Authentication/Extensions/AuthenticationExtensions.cs`.
- **Row-level isolation**: EF Core global query filters join through `XxxPermissions.UserId` → `Identity.SourceId` → `FinanceDbContext.CurrentUserId`, applied to every owned `DbSet` (`Finance.Persistence/Extensions/ModelBuilderExtensions.cs`, `AddQueryFilters`). This is the *only* access-control mechanism protecting financial data at read time — there is no field-level encryption to fall back on if it fails.
- **Infra-level key material**: ASP.NET Data Protection key rings under `.infra/*/dataprotection/*.xml`, mounted as a Docker volume. This protects auth cookies/antiforgery tokens for the frontend session — it is not used for, and does not touch, any database column.

### 2.2 What's missing

No field is encrypted, hashed, or masked anywhere in the backend (`grep -i "encrypt|hash|protect|aes"` across `Finance.Persistence`/`Finance.Domain` returns nothing relevant). Given §1, that's a defensible default for the `Money` columns — but the mechanism that currently stands in for protection (the query-filter ownership check) has a **critical, already-documented bug**:

- `FinanceDbContext.CurrentUserId` (`FinanceDbContext.cs:67`) reads `HttpContext.User.Identity.Name`, but the JWT pipeline sets `NameClaimType = ClaimTypes.NameIdentifier` and Auth0 tokens don't populate `ClaimTypes.Name`. Per `security-audit-backend-persistence-2026-03-29.md` (finding #7, **Critical**), `CurrentUserId` therefore resolves to the sentinel `"IdentityNotFound"` for real authenticated requests, and every ownership filter returns zero rows instead of the correct ones — masking data today, but leaving the system's only access-control layer unverified against a future change that fixes the symptom without understanding the cause. `FinanceDispatchContextBuilder` already reads `ClaimTypes.NameIdentifier` correctly elsewhere in the codebase; `CurrentUserId` should be aligned to it as the first, standalone fix, independent of the rest of this plan.
- Outside an HTTP request (seeder, migration, background job, test), `CurrentUserId` falls back to the same `"IdentityNotFound"` sentinel rather than throwing — silently correct-looking behavior (empty results) that would become actively dangerous once background processing (e.g. an event-sourcing projector) is introduced, since a projector legitimately needs to read/write across users and must not rely on this per-request mechanism at all.
- `Entity.Update(IEntity newData)` (domain audit finding, High) copies every settable property via reflection, including `UserId`/ownership fields on entities that expose them — a mass-assignment path that could let a crafted update payload change resource ownership.
- `ResourcePermissions.UserId` is a plain settable `Guid` with no invariant preventing reassignment after creation.
- No `CreatedBy`/`UpdatedBy` — `AuditedEntity<TId>` only has timestamps, so there's no way to attribute *who* changed a row from the DB alone; you'd have to correlate app logs.
- No optimistic concurrency tokens anywhere (no `RowVersion`/`xmin`), so concurrent updates to a `Fund` balance or `CreditCard` statement can silently clobber each other.
- No non-negativity `CHECK` constraints on any `Money` column, and `MoneyValueConverter` performs no round-trip validation, so a negative balance inserted via any bypass (seeder, migration, future bulk-import path) is silently accepted and returned as-is.
- `MoneyValueConverter` sets no precision/scale hints, so PostgreSQL maps every financial column to arbitrary-precision `numeric` rather than a bounded `numeric(18,4)`.
- `DbTelemetryInterceptor` emits full SQL text (including the `CurrentUserId`/Auth0 `sub` used in WHERE clauses) to OTel spans and slow-query logs — a PII leak into Jaeger/log aggregators.
- `DatabaseSeeder` logs the Auth0-fetched admin email at Information level (minor log-hygiene issue, not a stored-data issue since email isn't persisted).

None of these are new discoveries — they're pulled from the existing audits — but they matter directly here because **event sourcing amplifies whichever access-control and integrity story is already in place**: an event store is an append-only, harder-to-redact record, so it's worth closing the correctness gaps *before* building a durable log on top of the current model.

### 2.3 Data protection plan

Ordered by leverage, not by document position — do §1 regardless of whether event sourcing happens at all.

**1. Fix the ownership-filter claim bug (Critical, standalone, do first)**
Change `CurrentUserId` to read `ClaimTypes.NameIdentifier` via `User.FindFirst(...)`, matching `FinanceDispatchContextBuilder`. Add an integration test asserting an authenticated request resolves the real `sub`. Change the no-HTTP-context fallback from a sentinel string to an explicit `InvalidOperationException`, and thread `.IgnoreQueryFilters()` explicitly through the specific seeder/migration paths that legitimately need unfiltered access. This is a prerequisite for trusting *any* actor attribution later (including event-sourcing metadata).

**2. Close domain integrity gaps that would otherwise get "baked into" a permanent log**
- Add `HasCheckConstraint` non-negativity constraints on balance-bearing columns (`Fund.Amount`, `CreditCard.UnappliedCredit`, etc.) where the business rule genuinely requires it.
- Add `ConverterMappingHints(precision: 18, scale: 4)` to `MoneyValueConverter` (and its nullable variant).
- Remove the incorrect unique index on `Subscription.Price`.
- Add optimistic concurrency tokens (`xmin` is free on Postgres via `IsRowVersion()`) to entities with concurrent-write risk: `Fund`, `CreditCard`, `CreditCardStatement`.
- Constrain `Entity.Update(IEntity)` to an explicit allow-list of updatable properties per entity (or replace the reflection-based approach with generated/explicit update methods) so ownership fields can never be mass-assigned.

**3. PII field-level protection**
`Username`/`FirstName`/`LastName` are the only stored PII. Store them encrypted (AES-GCM) and keep them encrypted end-to-end through `Finance.Api` — the API's DTOs/mappers pass the ciphertext through unchanged, they never decrypt it. Decryption happens in exactly one place: `FinanceApp`'s server-side loaders (it's a React Router v7 SSR app — `AGENTS.md:14` — with a real Node server process, not a static/client-only app), right before rendering, so plaintext only ever exists transiently inside that one trusted process. `FinanceFunds` is an inactive, unused playground app (per user, not touched in months) and is out of scope — it isn't wired into this at all.

This means `Finance.Api` itself must not need plaintext for anything, including the existing `Username` uniqueness check — replace that with a comparison against a separate blind-index (HMAC) column instead of decrypting to compare. Keep `Identity.SourceId` unencrypted — it must remain queryable/indexed as the join key for the ownership filter, and it is already access-controlled the same way the rows it unlocks are.

Key handling: both `Finance.Api` (encrypts on write) and `FinanceApp` (decrypts on render) need the same key, added to each app's `secrets.local.env`/`.env` the same gitignored, runtime-injected way the Auth0 client secret and DB connection string already are (`.gitignore:2`, `:35`; `FinanceBackEnd/src/Finance.Api/Dockerfile:20-25`) — this is now a secret shared across two apps, not one. The AES-GCM implementation needs to match exactly between .NET (`Finance.Api`, write side) and Node/TS (`FinanceApp`, read side) — same key derivation, nonce/IV handling, and tag format — since one encrypts and the other decrypts the same ciphertext.

**4. Attribution**
Add `CreatedBy`/`UpdatedBy` (nullable `Guid` FK to `User`) to `IAuditedEntity`/`AuditedEntity<TId>`, populated from `FinanceDispatchContext.User` in the same `SaveChangesAsync` hook that already stamps `CreatedAt`/`UpdatedAt` (`SetAuditableDefaults`). This is also a direct dependency for event sourcing (§3) — every event needs an actor.

**5. Log/telemetry hygiene**
Strip `db.statement`/full SQL text from the `DbTelemetryInterceptor` OTel tag and slow-query log (replace with a hashed query identifier), and drop the email log line in `DatabaseSeeder`.

**6. Secrets hygiene**
Remove the unused `ApplicationOptions.ClientSecret` config to eliminate the risk of a real secret being committed under a dead key.

None of items 1–6 require new infrastructure — they're all changes within the existing `Finance.Persistence`/`Finance.Domain`/`Finance.Authentication` projects.

### 2.4 Open questions

- **Key architecture for PII encryption (item 3)**: whether `Finance.Api` and `FinanceApp` share the same symmetric AES-GCM key (simpler, but a `Finance.Api` compromise also exposes the decryption key — it would hold both the encrypt and decrypt capability), or an asymmetric/hybrid scheme where `Finance.Api` holds only a public key (used to encrypt, or to wrap a per-value data key) and `FinanceApp` alone holds the private key to decrypt — so compromising `Finance.Api` would never yield decryption capability. The asymmetric route needs either double encryption (envelope: a random per-value data key encrypts the value symmetrically, then that data key is wrapped with the public key) or support for multiple public keys if more than one recipient app ever needs decrypt access. Real reduction in blast radius on the encrypting side, at the cost of more moving parts. Needs a decision before implementing item 3.

---

## 3. Event sourcing feasibility

### 3.1 Should this app adopt event sourcing at all?

Partially, and incrementally — not as a wholesale rewrite. No domain events, outbox table, message bus, or background job runner exist today beyond the one-shot `DatabaseSeeder` hosted service, and the design in §3.3 doesn't need any of them: `event_stream` and every projected entity table live in the same Postgres database, so the event append and the projection update commit in one transaction — the exact case an outbox/message-bus pair exists to bridge, and there is no external system here for one to bridge to. The dispatcher itself is `CQRSDispatch`, an in-house replacement for an earlier MediatR-based dispatcher (per git history) — MediatR is a library, not a standard, and the project moved off it because its license changed to a paid commercial one. Any new event layer should extend `CQRSDispatch` with event-dispatch support rather than reintroduce a third-party mediator library, commercially licensed or otherwise.

Given the actual pain points this system has today (no audit trail, no way to answer "what was my fund balance on a given date," no protection against a bad statement import silently overwriting good data), full event sourcing of *every* aggregate is more machinery than the problem calls for. The better fit is a **hybrid**: keep the current tables as the system of record for entities that are reference/lookup data or rarely change, and introduce an append-only event log for the aggregates that are genuinely transactional ledgers, where "how did we get to this number" is a real question users and support will ask.

### 3.2 Candidate classification

| Treatment | Entities | Why |
|---|---|---|
| **Event-sourced** (append-only log is the source of truth; current table becomes a projection) | `Movement`, `Fund`, `Income`, `Debit`, `CreditCardTransaction`, `CreditCardPayment`, `CreditCardStatementAdjustment` | These are the actual ledger/balance-affecting entities. Users will want "why is my balance X" answered from history, imports need idempotent replay, and corrections should be new events, not silent overwrites. |
| **Event-sourced, lighter weight (append audit events, but table can stay canonical)** | `CreditCardStatement`, `Subscription`, `CurrencyExchangeRate` | State changes matter (a statement being re-closed, a subscription price changing, a rate being corrected) but these aren't high-frequency transactional writes — a simpler "record what changed" audit event is enough; full event-sourced rebuild is overkill. |
| **Stays plain CRUD** | `Bank`, `Currency`, `CurrencySymbol`, `CurrencyConversion`, `CreditCard`, `CreditCardIssuer`, `AppModule`, `AppModuleType`, `Frequency`, `IOLInvestment*`, `User`, `Identity`, `Role`, `ResourcePermissions<T>` | Reference/lookup data, identity/auth data, or entities where "current state" is the only thing that has ever mattered and history has no product value. Event-sourcing the ownership/permissions graph in particular would add real risk (a corrupted replay could misattribute financial data across users) for no corresponding benefit — access control should stay in the simplest, most auditable-by-inspection form it's already in. |

`IOLInvestment` is borderline — its `LastPrice`/`Valued` fields change frequently, but those changes come from an external market feed, not user action, so there's little value in owning that history inside this system versus just re-fetching from source; leave as CRUD unless a future requirement (e.g. performance-over-time charting) changes that calculus.

### 3.3 Proposed architecture

**Event store**: A single append-only Postgres table, e.g. `event_stream`, rather than a per-aggregate table — simplest to reason about and matches the existing single-DbContext, single-database deployment model. This is a plain EF-mapped table owned by `Finance.Migrations` like every other table, not a Marten-backed store. Marten owns its own schema/migration pipeline separate from `Finance.Migrations`, and writes through its own `IDocumentSession` — which would break the write-path atomicity below (event append + projection update in one `FinanceDbContext.SaveChangesAsync` transaction, no outbox); keeping a Marten session and an EF transaction atomic means manually sharing a connection or wrapping both in an ambient `TransactionScope`, which has known friction with Npgsql. Marten's real payoffs — async projection daemon, subscriptions, built-in snapshotting, a mature rebuild/replay pipeline — pay for a much larger event-sourcing surface than §3.2 scopes here, and would add a dependency and mental model the team hasn't used, on a codebase that already had to unwind one unwanted dependency (MediatR, see §3.1). So the table's columns are instead named to loosely mirror Marten's own `mt_events` shape, as a hedge: revisit Marten only if a real requirement emerges that this table can't satisfy (e.g. async projection rebuilds at scale, or external subscribers) — until then, a future migration would be a reshape-and-backfill rather than a redesign:

```
event_stream
  seq_id          bigserial primary key       -- global ordering across all streams (Marten: seq_id)
  id              uuid        not null         -- event id (Marten: id)
  stream_id       uuid        not null         -- aggregate id, e.g. FundId (Marten: stream_id)
  stream_type     varchar     not null         -- "Fund", "Movement", ... (kept for query convenience; not a direct Marten column)
  version         int         not null         -- per-stream sequence, for optimistic concurrency (Marten: version)
  type            varchar     not null         -- "fund_balance_adjusted", "movement_recorded", ... (Marten: type)
  data            jsonb       not null         -- event-specific payload (Marten: data)
  metadata        jsonb       not null         -- actor (UserId, from item #4 in §2.3), correlation id, source — Marten's closest analog is its `headers` column
  timestamp       timestamptz not null default now()  -- (Marten: timestamp)
  unique (stream_id, version)
```

This is "Marten-adjacent," not "Marten-compatible" — Marten also tracks stream-level metadata in a companion `mt_streams` table (aggregate type, current version, archived flag) and a `.NET` type discriminator for polymorphic deserialization, neither of which this table needs today. Matching column names and shape is a cheap hedge against a possible future migration; it is not a guarantee of a drop-in swap.

JSONB payload keeps this schema-flexible without a migration per new event type, and Postgres's native JSONB indexing/`LISTEN`/`NOTIFY` covers query and near-real-time notification needs without introducing a separate message-bus dependency.

**Aggregate/event conventions** (mirroring existing `CQRSDispatch` naming, under a new `Finance.Domain/Events/<Module>/` + `Finance.Application/Events/<Module>/` split, matching the "category-first, then domain" folder convention already used for Commands/Queries):
- `IDomainEvent` marker interface with `StreamId`, `OccurredAt`.
- Concrete events per module, e.g. `FundBalanceAdjusted`, `MovementRecorded`, `MovementCorrected`, `CreditCardTransactionPosted`, `CreditCardPaymentApplied`.
- An `IEventHandler<TEvent>` applied the same way `ICommandHandler`/`IQueryHandler` are today, registered through the same assembly-scan `CommandHandlerTypeRegistry`-style mechanism.

**Write path**: Extend the existing `SaveChangesAsync` hook pattern in `FinanceDbContext` (alongside `AutoCreateOwnershipPermissions`/`SetAuditableDefaults`) with a third hook, `CaptureDomainEvents`, that inspects tracked entities of the event-sourced types and appends the corresponding event row(s) in the same transaction as the entity write — keeping the projection and the event log atomically consistent with no outbox needed. An outbox becomes necessary only if something outside Postgres (a queue, another service) needs to be notified.

**Read path / projections**: The existing tables (`Fund`, `Movement`, etc.) remain queryable exactly as they are today — they become projections that get written to as a side effect of event capture, not something callers stop using. This is the key design choice that keeps this incremental: no query, controller, or frontend code needs to change on day one. History/audit views are additive new queries against `event_stream` filtered by `stream_id`/`stream_type`.

**Concurrency**: `version` + the `unique(stream_id, version)` constraint gives optimistic concurrency for free — a concurrent write attempting to append at a stale version fails the DB constraint, which doubles as the fix for the missing-concurrency-token gap noted in §2.3 item 2 for event-sourced entities specifically.

**Snapshotting**: Not needed initially — none of these streams are expected to reach a length where full-stream replay is slow (a `Fund`'s history is bounded by user transaction volume, not unbounded IoT-scale event rates). Revisit if replay performance becomes a measured problem.

**Corrections & erasure**: Because the log is append-only, editing a past `Movement` appends a new `MovementCorrected` event referencing the original, in `event_stream` only — the `Movement` row itself is still updated in place, same as any edit today, so nothing changes for the user or the frontend. The correction event exists purely as the backend-internal record of what happened. For GDPR-style erasure requests (a `User` wants their data gone), plan for **crypto-shredding**: encrypt event payloads containing that user's data with a per-user data key, and erasure becomes "destroy the key" rather than rewriting immutable history. This should be designed alongside the PII encryption work in §2.3, not bolted on later.

### 3.4 Rollout phases

1. **Prerequisite hardening** — §2.3 items 1, 2, 4 (claim-bug fix, integrity constraints, `CreatedBy`/`UpdatedBy`). Ship independently of event sourcing; needed regardless.
2. **Event capture, no behavior change** — add `event_stream`, the `SaveChangesAsync` hook, and events for one low-risk aggregate first (`Fund` is a good pilot: high user value for "balance history," bounded write volume). Existing reads/writes unaffected; only additive.
3. **Expose history** — new read-only queries/endpoints surfacing `Fund` event history (e.g. "balance over time" chart), validating the model against a real feature before expanding.
4. **Expand to remaining event-sourced aggregates** from §3.2's first row, one at a time, reusing the same hook/handler pattern.
5. **Corrections-as-events** — extend the existing edit flows for event-sourced aggregates to also append a correction event alongside the existing in-place row update; no UI or behavior change, purely additive to the event log.
6. **PII encryption + crypto-shredding for erasure** (§2.3 item 3 + §3.3 erasure note) — can happen in parallel with 2–4, since it's largely independent of the event-store mechanics.

