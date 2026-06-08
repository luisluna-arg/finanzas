---
name: security-review-backend
description: Analyze the backend architecture and data persistence layer to identify security vulnerabilities in the handling of financial information.
---

## Stack Context

- **Language / Framework**: C# / ASP.NET Core Web API
- **ORM**: Entity Framework Core with global query filters for ownership isolation
- **Authentication**: Auth0 JWT — `user_id` / `sub` claim is the identity anchor
- **Infrastructure**: PostgreSQL, Redis, Docker

## Where to Look

Focus your search on these file patterns:

- `*Controller.cs` — authorization enforcement, model binding, input validation
- `*Handler.cs` / `*Service.cs` — business logic, data access, ownership filters
- `*Configuration.cs` / `*EntityTypeConfiguration.cs` — EF column mappings (field types, encryption)
- `*DbContext.cs` — global query filters, ownership constraints
- `appsettings*.json`, `Program.cs` — middleware pipeline, logging sinks, secret handling

## Analysis Guidelines

### Encryption at Rest

- Identify numeric or text fields representing monetary values (e.g. `balance`, `amount`, `salary`) and verify they are not stored in plain text in the database.
- Detect missing symmetric encryption (e.g. AES-256) for sensitive financial columns in EF configurations.

### Identity Isolation (Auth0 Integration)

- Verify that the `user_id` / `sub` from the Auth0 token is the sole link to financial records.
- Ensure no tables directly associate personally identifiable data with balances without an abstraction layer.
- Check that global EF query filters enforce ownership and cannot be bypassed (e.g. via `IgnoreQueryFilters()`).

### Object-Level Authorization (BOLA)

- Audit controllers and handlers to confirm every query filters strictly by the authenticated user's ID.
- Flag any endpoint that accepts a user or resource ID from the request body or URL without re-validating ownership against the token claim.

### Mass Assignment

- Detect model binding that accepts raw domain entities or overly permissive DTOs, allowing callers to set fields like `UserId`, `Balance`, or `Role` directly.

### Log and Exception Leakage

- Verify that error-handling and logging routines do not capture or serialize objects containing financial amounts to server logs.
- Check that exception middleware does not return stack traces or internal model data to API consumers.

### Rate Limiting

- Flag financial write endpoints (transfers, payments, balance updates) that lack rate limiting or idempotency controls.

### Integrity Validation

- Detect missing business-logic validations (e.g. allowing negative amounts where not permitted, or numeric overflow in financial calculations).
- Check that decimal precision is enforced at the domain level, not only at the database column level.

## Output Instructions

For each risk found, report:

| Field | Detail |
|---|---|
| **File** | Path to the affected file |
| **Line** | Line number(s) |
| **Description** | Technical description of the vulnerability |
| **Priority** | Critical / High / Medium |
| **Recommendation** | Suggested fix or mitigation |

## Report File

Save the report as a Markdown file under:

```
docs/security-audits/security-audit-{scope}-YYYY-MM-DD.md
```

- Use today's date in the filename.
- `{scope}` identifies what was reviewed. Use `backend` for a general review or a more specific segment when the review is scoped (e.g. `backend-authentication`, `backend-persistence`, `backend-application-commands`, `backend-application-queries`, `backend-domain`).
- If a file for today with the same scope already exists, append a counter (e.g. `-2`).