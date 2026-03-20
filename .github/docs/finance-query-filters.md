# EF Query Filters & Test Identity Seeding

- `FinanceDbContext` applies global ownership filters to `Fund` and `Movement`.
- In tests with no `HttpContextAccessor`, `CurrentUserId` defaults to `IdentityNotFound`.
- Query tests must seed a `User` identity with `SourceId = IdentityNotFound` plus matching `FundPermissions` / `MovementPermissions` to make entities visible.
