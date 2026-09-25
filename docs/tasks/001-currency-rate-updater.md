# Task: Currency Rate updater

## Context & Objective

This feature will enable funds front end to automatically get the latest currency exchange rate for different currencies using an external provider

## Restrictions

- This task is code only, do not attempt to access any environment for testing
- Only unit tests are allowed to be written, for backend and/or frontend — do not run them
- This task will only change the shared .NET backend `FinanceBackEnd` and the `FinanceFrontEnd\FinanceFunds` project
- The `FinanceFrontEnd\FinanceApp` project should be excluded from this work
- The as final step the new feature should be documented under `\docs\architecture`
- All code must go through review before anything else is run — only make code changes (edit/create files) and compilation checks (e.g. `tsc`/`dotnet build`) to verify the code builds; do not run tests, migrations, or any other command

## Requirements

### Backend

#### Provider

Provider management will allow the creation of a provider for the currency exchange rate.
Each provider will have:
- a name
- a URL
- an HTTP method
- A response template and mappers to access buy and sell rate values in that template
- A link to the `CurrencyPair` it provides rates for (see Association below) — the provider does not store its own base/quote currency fields

##### Example

Provider url: https://dolarapi.com/v1/dolares/tarjeta

Provider http method: "Get" - {Define a proper way to support "POST" since it would require body parameters}

Provider response - Can be stored as json field

```json
{
  "moneda": "USD",
  "casa": "tarjeta",
  "nombre": "Tarjeta",
  "compra": 1930.5,
  "venta": 1995.5,
  "fechaActualizacion": "2026-09-23T18:00:00.000Z"
}
```

The response template could specify expected types

```json
{
  "compra": { "type": "Double" },
  "venta": { "type": "Double" },
  "casa": { "type": "string" },
  "nombre": { "type": "string" },
  "moneda": { "type": "string" },
  "fechaActualizacion": { "type": "datetime", "format": "YYYY-MM-DDTHH:mm:ss.sssZ" }
}
```

#### Association

The provider should be associated to currencies. This link should follow these rules:

- The currency exchange rate's currency pair should now just store the two currencies, becoming a strong entity on its own — `CurrencyPair` — with a compound key of the two currencies
- The providers should be linked to a `CurrencyPair`
- Providers and `CurrencyPair` are per-user resources, owned the same way as every other user-owned entity in the system — apply the same ownership/permission model to both, and update the currency exchange rate's existing ownership to match the entity split
- Actual exchange rates should now point to `CurrencyPair` instead of including the two currencies on each record
- A `CurrencyPair` can still have no providers
- A `CurrencyPair` can have multiple providers
- Exactly one provider must be active at a time per `CurrencyPair`, unless it has no providers left, in which case none is active
  - At most one active provider per `CurrencyPair`, enforced at database level
  - Enforce this via database, and on backend create, update or delete commands
- Exchange rate records should include a nullable field indicating the provider that produced that record's rate values
- Exchange rate records without provider should be considered manual at codebase level
- Providers can be soft deleted
- When the active provider for a `CurrencyPair` is soft deleted, activate the next available provider for that `CurrencyPair`, round-robin

#### New endpoint to request an update

- API should include an endpoint to trigger an update for a `CurrencyPair`
- If there are no providers configured return the error indicating it
- When update is completed, the response should return the updated values using the project's DTO for currency exchange rates
- Consider that production environment may not be available to make external requests so backend may require special configurations to enable that, document requirements that exceed infra config other that what you can change in this repo as a markdown file under `\docs\architecture`

### Frontend

The Funds project `/exchange-rates` route should now include a new button to get an update

- New button should be located next to `Add new exchange rate`
- Responsive design that adapts mainly for cell phone screens and also to PC web browsers
- New button should open a modal that allows the selection of the `CurrencyPair` and shows the provider being used (Name)
- If there is no available provider the `CurrencyPair` should not be selectable, only `CurrencyPair`s with a provider can be used (There will be a separate admin section to maintain this)
- The table for currencies should now include filtering for both currencies
- Each row in the table should have a refresh button to use the active provider to get an update and refresh on the fly (Without reloading the page, just the row)
- Funds should now have a section to maintain the configurable aspects of this new feature, scoped to the current user like every other resource in the app
    - Currencies
    - Currency exchange rate (`CurrencyPair`)
    - Providers, including deletion — the frontend only triggers the delete call, active-provider reassignment (round-robin) is backend logic, not frontend

## Acceptance Criteria

### Backend — `CurrencyPair` entity & migration

- [ ] `CurrencyPair` entity exists as its own strong entity, keyed by the two currencies (base + quote)
- [ ] `CurrencyPair` is a per-user resource, using the same ownership/permission model as other user-owned entities
- [ ] A data migration backfills a `CurrencyPair` for every distinct currency pair present in existing exchange rate records
- [ ] The data migration re-points existing exchange rate records from their inline currency fields to the matching `CurrencyPair` (no data loss)
- [ ] The data migration carries over ownership from the existing currency exchange rate records to the corresponding `CurrencyPair`
- [ ] Exchange rate records reference `CurrencyPair` (via a foreign key — compound or single-value, per the entity's actual key implementation) instead of storing both currencies directly on each record
- [ ] Current currency exchange rate records should no longer have owner, because that is now transferred to the `CurrencyPair`

### Backend — Provider & association rules

- [ ] A Provider can be created with a name, URL, HTTP method, and a response template with mappers for buy/sell rate values
- [ ] A Provider's response template can specify expected field types (e.g. Double, string, datetime)
- [ ] A Provider is linked to a `CurrencyPair` — it does not store its own base/quote currency fields
- [ ] Providers are per-user resources, using the same ownership/permission model as other user-owned entities
- [ ] A `CurrencyPair` can exist with zero providers linked
- [ ] A `CurrencyPair` can have multiple providers linked
- [ ] Exactly one provider is active per `CurrencyPair` whenever it has at least one provider; none is active when it has none
- [ ] A database-level constraint prevents more than one active provider per `CurrencyPair`
- [ ] Create, update, and delete commands enforce the active-provider invariant
- [ ] Providers can be soft deleted
- [ ] The soft-delete command automatically promotes the next available provider (round-robin) when the deleted provider was the active one for its `CurrencyPair`; if no other provider is left, the `CurrencyPair` is left with no active provider
- [ ] Each exchange rate record has a nullable field identifying the provider that produced its values
- [ ] Exchange rate records with no provider are treated as manual entries

### Backend — Update endpoint

- [ ] An API endpoint exists to trigger a rate update for a given `CurrencyPair`
- [ ] Requesting an update for a `CurrencyPair` with no providers configured returns an error indicating that
- [ ] A successful update returns the updated values using the project's existing currency exchange rate DTO
- [ ] Any infra requirements beyond what's configurable in this repo (e.g. outbound network access from production) are documented as a markdown file under `\docs\architecture`

### Frontend

- [ ] `/exchange-rates` has a new "get an update" button next to "Add new exchange rate", this button should have a refresh icon and a tooltip 
- [ ] The new UI is responsive on both mobile and desktop screen sizes
- [ ] The button opens a modal to select a `CurrencyPair` and shows the name of the provider that will be used
- [ ] `CurrencyPair`s with no available provider are not listed in the modal
- [ ] The currency table supports filtering by both currencies in the pair
- [ ] Each table row has a refresh button that updates only that row's rate via its active provider, without a full page reload
- [ ] A new admin section exists to manage Currencies, `CurrencyPair`s, and Providers, scoped to the current user
- [ ] That section allows deleting a Provider with a confirmation modal before executing

### Process

- [ ] An implementation plan is drafted as first step, the agent should iterate until there are no inconsistencies. This plan should then be stored under `docs\tasks`
- [ ] The feature is documented under `\docs\architecture` as a final step
- [ ] Only unit tests are added (backend and/or frontend) — no other test types
- [ ] No changes are made to `FinanceFrontEnd\FinanceApp`
- [ ] No environment access is attempted for testing (code-only task)
