---
name: security-review-frontend
description: Analyze frontend code to identify financial data leakage and session management vulnerabilities.
---

## Stack Context

- **Frameworks**: React Router v7 (SSR, `FinanceApp`) and Vite SPA (FinanceFunds)
- **Authentication**: Auth0 SDK (`@auth0/auth0-react`)
- **HTTP client**: Axios or `fetch` with interceptors
- **State**: React Context / component-local state

## Where to Look

Focus your search on these file/folder patterns:

- `app/routes/` — route components, loaders, and actions (SSR data exposure)
- `app/components/` — rendering of financial values and dynamic API content
- `src/hooks/`, `src/context/` — token and state management
- `src/api/`, `src/services/` — HTTP interceptors, request/response logging
- `app/root.tsx`, `app/entry.server.tsx` — SSR serialization into HTML

## Analysis Guidelines

### Data Exposure in DOM / State

- Detect whether financial amounts (`balance`, `total_income`) are stored in component state or context beyond the lifetime of the view that needs them.
- Verify that sensitive data is not rendered in visible HTML attributes (`data-*`, `title`, `alt`).
- In SSR routes (React Router loaders), check whether financial data returned from loaders is serialized into the initial HTML payload more broadly than necessary.

### Local Storage Handling

- Audit use of `localStorage` and `sessionStorage`. Flag any storage of balances, amounts, or access tokens that would be readable by XSS.
- Verify that no financial data remains in browser storage or cache after logout.

### Session Security (Auth0 Integration)

- Review the Auth0 SDK configuration: ensure PKCE is used and `cacheLocation` is set to `memory`, not `localstorage`.
- Verify that Access Token / ID Token values are never logged via `console.log` or included in error reporting payloads.

### Injection Vulnerabilities (XSS)

- Search for `dangerouslySetInnerHTML`, `innerHTML`, and `eval()`. Any use with API-sourced data (expense descriptions, category names, notes) is a critical risk.
- Flag string interpolation into URLs or `href` attributes without sanitization.

### Network / Console Exposure

- Detect Axios interceptors or `fetch` wrappers that log full response bodies (which contain financial data) to the console without a production guard.
- Verify that error boundaries and reporting tools (e.g. Sentry) are configured to scrub financial fields before transmission.

### Content Security Policy

- Check whether a `Content-Security-Policy` header or meta tag is configured to restrict script sources and mitigate XSS impact.

## Output Instructions

For each risk found, report:

| Field | Detail |
|---|---|
| **Component / File** | Path to the affected file or component |
| **Line** | Line number(s) |
| **Impact** | Potential impact of the vulnerability |
| **Priority** | Critical / High / Medium |
| **Recommendation** | Suggested fix or mitigation |

## Report File

Save the report as a Markdown file under:

```
docs/security-audits/security-audit-{scope}-YYYY-MM-DD.md
```

- Use today's date in the filename.
- `{scope}` identifies what was reviewed. Use `frontend` for a general review or a more specific segment when the review is scoped (e.g. `frontend-financeapp-routes`, `frontend-financefunds-auth`).
- If a file for today with the same scope already exists, append a counter (e.g. `-2`).