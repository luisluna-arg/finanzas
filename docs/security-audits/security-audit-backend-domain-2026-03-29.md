# Security Audit — Finance.Domain

**Date**: 2026-03-29
**Scope**: `Finance.Domain/Models/**`, `Finance.Domain/SpecialTypes/Money.cs`, `Finance.Domain/Policies/CurrencyConversionPolicy.cs`, `Finance.Domain/Comparers/MovementComparer.cs`, `Finance.Domain/JsonConverters/**`
**Reviewer**: GitHub Copilot

---

## Findings

### 1. `Entity.Update()` Reflection-Based Mass Assignment of All Properties — Including Ownership Fields

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Domain/Models/_Base/Entity.cs](FinanceBackEnd/src/Finance.Domain/Models/_Base/Entity.cs#L12) |
| **Lines** | [L12–L16](FinanceBackEnd/src/Finance.Domain/Models/_Base/Entity.cs#L12) |
| **Description** | `Entity.Update(IEntity newData)` reflects over every readable and writable property of `newData` and copies values to `this`. Because it sets all properties generically, it will copy `Id`, `Deactivated`, and any ownership-bearing fields (e.g. `UserId` on permission entities, `BankId`, `CurrencyId`) directly from whatever caller-supplied object is passed. Any command handler that deserializes user input into a domain entity (or even a DTO that is subsequently mapped to one) and then calls `entity.Update(input)` creates a mass assignment vector. An authenticated user who crafts a request with a different `UserId` GUID could silently hijack another user's resource record by overwriting its ownership field in a single `Update` call — with no handler-level knowledge that this happened. |
| **Priority** | High |
| **Recommendation** | Remove the reflection-based universal `Update`. Replace with explicit update methods on each entity that accept only the domain-safe properties callers are allowed to change (e.g. `MoveTimestamp`, `SetAmount`). If a generic helper is required, use an allowlist of property names rather than a denylist, and explicitly exclude `Id`, `UserId`, `Deactivated`, and all foreign-key ownership fields. |

---

### 2. `Money` Implicit Casts to Integer Types Silently Truncate Decimal Financial Amounts

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Domain/SpecialTypes/Money.cs](FinanceBackEnd/src/Finance.Domain/SpecialTypes/Money.cs#L43) |
| **Lines** | [L43–L45](FinanceBackEnd/src/Finance.Domain/SpecialTypes/Money.cs#L43) |
| **Description** | `Money` defines three implicit narrowing conversions: `implicit operator short`, `implicit operator int`, and `implicit operator long`. Each calls `Convert.ToInt16/32/64(value)`, which silently truncates the fractional part of a decimal financial amount (e.g. `$1234.99` becomes `1234`). Because these conversions are implicit, the compiler will not warn when a `Money` value is passed to a method expecting `int`, assigned to an integer field, or used in an integer expression. This can silently corrupt calculated balances, totals, or transaction amounts at any call site without raising an exception. |
| **Priority** | High |
| **Recommendation** | Remove the implicit `operator short`, `operator int`, and `operator long` conversions. If integer representations are ever needed (e.g. for external serialization), replace them with explicit methods (e.g. `ToInt64()`) so the truncation is a deliberate, visible operation at every call site. Prefer keeping `Money → decimal` as the only implicit path and requiring explicit casts for all integer conversions. |

---

### 3. `CurrencyExchangeRate.BuyRate` Defaults to `0m` — Division by Zero in `CurrencyConversionPolicy`

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Domain/Policies/CurrencyConversionPolicy.cs](FinanceBackEnd/src/Finance.Domain/Policies/CurrencyConversionPolicy.cs#L9) |
| **Lines** | [L9](FinanceBackEnd/src/Finance.Domain/Policies/CurrencyConversionPolicy.cs#L9) |
| **Description** | `CurrencyConversionPolicy.Apply` divides `amount / currencyExchangeRate.BuyRate` when `BaseCurrencyId == sourceCurrencyId`. `CurrencyExchangeRate.BuyRate` is declared as `Money BuyRate { get; set; } = 0m` ([CurrencyExchangeRate.cs L14](FinanceBackEnd/src/Finance.Domain/Models/Currencies/CurrencyExchangeRate.cs#L14)) and defaults to zero. Dividing a `decimal` by `0m` throws `DivideByZeroException`. There is no domain-level validation rejecting a zero or negative exchange rate at either the entity level or in the policy itself, so any persisted `CurrencyExchangeRate` with a zero buy rate will cause an unhandled exception at conversion time. |
| **Priority** | High |
| **Recommendation** | Guard against zero and negative rates in the `CurrencyConversionPolicy` before performing division: throw a descriptive domain exception (`InvalidOperationException("Exchange rate BuyRate must be greater than zero")`) rather than letting a `DivideByZeroException` propagate. Additionally, enforce the constraint at the domain level: add a constructor or factory method on `CurrencyExchangeRate` that requires `buyRate > 0`, or add a validation attribute enforced by the command handler before persisting. |

---

### 4. `Money` Struct Accepts Negative Values — No Domain-Level Non-Negativity Guard

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Domain/SpecialTypes/Money.cs](FinanceBackEnd/src/Finance.Domain/SpecialTypes/Money.cs#L7) |
| **Lines** | [L7–L9](FinanceBackEnd/src/Finance.Domain/SpecialTypes/Money.cs#L7) |
| **Description** | `Money(decimal value)` places no constraint on the sign of `value`. Every financial entity in the domain (`Income.Amount`, `Fund.Amount`, `Debit.Amount`, `CreditCardTransaction.Amount`, `CreditCardPayment.Amount`, `Movement.Amount`) uses `Money` with no additional guard. A caller who submits a negative amount (e.g. `"amount": -50000`) for an income or fund deposit passes through domain construction unchallenged. Business rules that should prevent negative income or negative fund balances are entirely absent at the domain level and must be — but currently are not reliably — in every individual command handler. |
| **Priority** | Medium |
| **Recommendation** | Introduce a factory method or secondary constructor — e.g. `Money.NonNegative(decimal value)` — that throws `ArgumentOutOfRangeException` when `value < 0`, and use it where non-negative domain semantics apply. For signed amounts where negative values are legitimate (e.g. adjustment entries), document the intent explicitly in the entity. This moves the invariant to the domain layer rather than trusting every command handler in perpetuity. |

---

### 5. `MovementComparer.Equals()` Violates Symmetry — Equality Is Asymmetric

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Domain/Comparers/MovementComparer.cs](FinanceBackEnd/src/Finance.Domain/Comparers/MovementComparer.cs#L13) |
| **Lines** | [L11–L17](FinanceBackEnd/src/Finance.Domain/Comparers/MovementComparer.cs#L11) |
| **Description** | `MovementComparer.Equals(x, y)` includes the condition `x.TimeStamp.Date.Subtract(y.TimeStamp.Date).TotalDays == 1`, which evaluates to `true` only when `x` is exactly one calendar day *before* `y`. By contrast, `Equals(y, x)` would compute `y.TimeStamp - x.TimeStamp = -1`, which is `!= 1`, so it returns `false`. This violates the `IEqualityComparer<T>` symmetry contract: `Equals(a, b)` must equal `Equals(b, a)`. Any deduplication of imported bank movements using this comparer (e.g. with `HashSet<Movement>` or `Distinct()`) will silently produce different results depending on insertion order, potentially resulting in duplicate transactions being stored or valid movements being discarded. |
| **Priority** | High |
| **Recommendation** | Determine the intended semantic (are "adjacent-day" movements to be considered equal, or same-day?). If same-day is the intent, replace the condition with `x.TimeStamp.Date == y.TimeStamp.Date`. If "within 1 day" is the intent, use `Math.Abs((x.TimeStamp.Date - y.TimeStamp.Date).TotalDays) <= 1`. Either way, ensure the `GetHashCode` implementation returns the same value for any two movements that `Equals` considers equal — currently `GetHashCode` hashes the full `TimeStamp`, so two movements on adjacent days would have different hash codes but the same `Equals` result, further breaking hash-based collection invariants. |

---

### 6. `CreditCard.UnappliedCredit` and `IOLInvestment.DailyVariation` Bypass the `Money` Type

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Domain/Models/CreditCards/CreditCard.cs](FinanceBackEnd/src/Finance.Domain/Models/CreditCards/CreditCard.cs#L10) |
| **Lines** | [L10](FinanceBackEnd/src/Finance.Domain/Models/CreditCards/CreditCard.cs#L10), [IOLInvestment.cs L15](FinanceBackEnd/src/Finance.Domain/Models/IOLInvestments/IOLInvestment.cs#L15) |
| **Description** | `CreditCard.UnappliedCredit` is typed as plain `decimal`, and `IOLInvestment.DailyVariation` is also `decimal`, while every other financial amount in the domain uses the `Money` value type. This inconsistency means: (1) These fields are not subject to the `MoneyJsonConverter` or `MoneyNewtonsoftJsonConverter` serialization rules, so API consumers may receive or send them in formats inconsistent with other monetary amounts. (2) Any future domain-level validation added to `Money` (see finding 4) will not apply to these fields. (3) The EF column type/precision configuration for `Money` fields may differ from these plain `decimal` fields depending on the persistence configuration, creating inconsistent database storage. |
| **Priority** | Medium |
| **Recommendation** | Change `CreditCard.UnappliedCredit` to `Money` and `IOLInvestment.DailyVariation` to `Money` (or, if `DailyVariation` is a percentage, a purpose-built `Percent` value type) to maintain consistency with the rest of the domain. Update the EF configuration and persistence layer accordingly. |

---

### 7. `ResourcePermissions.UserId` Has No Domain-Level Ownership Constraint

| Field | Detail |
|---|---|
| **File** | [FinanceBackEnd/src/Finance.Domain/Models/Auth/ResourcePermissions.cs](FinanceBackEnd/src/Finance.Domain/Models/Auth/ResourcePermissions.cs#L19) |
| **Lines** | [L17–L20](FinanceBackEnd/src/Finance.Domain/Models/Auth/ResourcePermissions.cs#L17) |
| **Description** | `ResourcePermissions<TResource, TResourceId>` exposes `UserId` as a plain settable `Guid` with no encapsulation (`public Guid UserId { get; set; } = default!`). The domain model itself enforces no invariant that `UserId` must correspond to an authenticated user. `Guid.Empty` (the C# default) is structurally valid and could be persisted if a command handler fails to assign the correct user ID — silently producing a permission record owned by nobody, or one that is invisible to EF global query filters that compare by `UserId`. This is purely a defence-in-depth concern since handlers currently set `UserId` correctly, but the domain offers no safety net. |
| **Priority** | Medium |
| **Recommendation** | Make `UserId` constructor-required: add a protected constructor that takes `Guid userId` and throws `ArgumentException` if `userId == Guid.Empty`. Expose it only via a factory method such as `ResourcePermissions.ForUser(Guid userId, TResourceId resourceId)`. This makes it structurally impossible to create a detached permission record, regardless of what command handlers do. |
