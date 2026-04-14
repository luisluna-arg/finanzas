# Frontend Architecture — FinanceApp

## Overview

FinanceApp is a React Router v7 application running in SSR mode (formerly Remix). It authenticates users via Auth0, stores session tokens server-side in Redis, and loads all initial page data in server-side `loader` functions. Access tokens never reach the browser. A server-side proxy route (`/api/proxy`) is the only sanctioned path for client-side fetches.

Port: `5100`.

---

## Tech Stack

| Concern | Choice |
|---|---|
| Framework | React Router v7 (SSR) |
| React | v18 |
| Build tool | Vite via `@react-router/dev` |
| Styling | Tailwind CSS v3 + `tailwindcss-animate` |
| Component library | shadcn/ui (Radix UI primitives + `cva`, `clsx`, `tailwind-merge`) |
| HTTP client | Axios |
| Forms | `react-hook-form` + `@hookform/resolvers` + Zod v4 |
| Client-side data | `react-query` v3 (`useQuery` / `useInfiniteQuery`) |
| Auth | `remix-auth` + `remix-auth-auth0`, JWT verification via `jose` |
| Session store | Redis (`ioredis`), in-process `MockRedis` fallback for local dev |
| Charts | `recharts` |
| Tables | `@tanstack/react-table` |
| Date handling | `date-fns`, `dayjs`, `react-day-picker` |
| Logging | `pino` + `pino-http` |
| Observability | OpenTelemetry (OTLP HTTP, fetch/http/undici instrumentation) |

---

## Project Structure

```
app/
  routes/             # File-based route modules
  components/
    ui/               # Feature-scoped UI components (Dashboard, CreditCards, …)
    ui/shadcn/        # Generated shadcn/ui wrappers
    stores/           # paginatedQuery.tsx — react-query client-side hook
    data/             # InputControlTypes
  data/               # Server-side data access layer
    BackendClient.ts       # Facade owning all Query instances
    getBackendClient.ts    # Factory
    base/                  # BaseQuery, BasePaginatedQuery, axiosConfig
    queries/               # One class per resource (BanksQuery, CreditCardQuery, …)
  services/
    auth/             # Session, token refresh, auth cookies (all .server.ts)
  hooks/              # use-mobile.ts
  lib/                # utils.ts (cn helper)
  middleware/         # logger.server.ts (pino-http)
  telemetry/          # tracing.server.ts, tracing.client.ts
  types/              # Domain TypeScript interfaces
  utils/              # BackendUrl, urls, dates, currency constants, loggers, SafeLogger
  root.tsx            # App shell — global loader, navigation render
  entry.server.tsx    # SSR entry — OTel init, renderToPipeableStream, request logging
  entry.client.tsx    # Hydration entry — browser OTel init, hydrateRoot
```

---

## Routing

Routes are discovered via `@react-router/fs-routes` `flatRoutes()` (configured in `app/routes.ts`). Dot-notation filenames map directly to URL segments and layout nesting.

| File | URL / Role |
|---|---|
| `auth.login.tsx` | `/auth/login` |
| `auth.auth0.tsx` | `/auth/auth0` — initiates OAuth redirect |
| `auth.callback.tsx` | `/auth/callback` — OAuth callback, creates session |
| `auth.logout.tsx` | `/auth/logout` — destroys session |
| `auth.forbidden.tsx` | `/auth/forbidden` |
| `dashboard.tsx` | `/dashboard` |
| `dashboard.summary.tsx` | `/dashboard/summary` |
| `credit-cards.tsx` | Layout shell (renders `<Outlet />`) |
| `credit-cards._index.tsx` | `/credit-cards` (index) |
| `credit-cards.statement.$id.tsx` | `/credit-cards/statement/:id` |
| `subscriptions.tsx` | `/subscriptions` |
| `incomes.tsx` | `/incomes` |
| `investments.tsx` | `/investments` |
| `funds.tsx` | `/funds` |
| `currency-exchange-rates.tsx` | `/currency-exchange-rates` |
| `api.proxy.ts` | `/api/proxy` — server-side proxy for client fetches |
| `health.ts` | `/health` — dependency-free health check |

`root.tsx` wraps everything and renders `<Navigation>` when the user is authenticated.

---

## Data Loading

All initial data fetching is server-side, in route `loader` functions. The pattern is consistent across every protected route:

```
loader
  → requireAuth(request)         // reads Redis session, verifies JWT
  → getBackendClient(accessToken) // instantiates BackendClient
  → client.GetXxxQuery().get()   // axios call to backend
  → return data                  // serialised to component via useLoaderData()
```

**`BackendClient`** owns one instance of each `Query` class and is created per request with the current access token.

**`BaseQuery` / `BasePaginatedQuery`** are axios-based. `buildAxiosConfig` injects `Authorization: Bearer <accessToken>`.

**`BackendUrl`** is a builder with two modes:
- `.toRaw()` — direct backend URL; used by server-side loaders.
- `.toString()` — `/api/proxy?path=...`; used for client-side `react-query` calls.

**API base URL resolution** (`app/utils/urls.ts`):
- Server: `process.env.API_URL` (fallback: `http://localhost:5000`).
- Client: derived from `window.location.hostname`.

**Parallel loading**: `Promise.all([…])` is used in loaders with multiple backend calls (e.g. dashboard, credit cards index) to avoid waterfall fetches.

---

## Authentication

**Provider**: Auth0 via `remix-auth` + `remix-auth-auth0`.

### OAuth Flow

1. `POST /auth/auth0` → `authenticator.authenticate()` — starts Auth0 redirect.
2. Auth0 redirects to `GET /auth/callback` → code exchange, receives `{ accessToken, refreshToken, idToken }`.
3. `createUserSession(user, '/dashboard')`:
   - Stores the token set in Redis under `serverSession:<uuid>`.
   - Writes only the UUID into the `__user_session` httpOnly cookie (7-day expiry, `sameSite: lax`).

### Session Validation

`requireAuth(request)` (`app/services/auth/session.server.ts`):
1. Reads UUID from `__user_session` cookie.
2. Fetches token set from Redis.
3. Verifies the ID token with `jose` against Auth0 JWKS (`/.well-known/jwks.json`).

### Token Refresh

`tokenRefresh.server.ts` decodes the JWT, checks `exp`, and on expiry calls Auth0 `/oauth/token` with the refresh token. The Redis entry is updated with the new token set. Refresh is triggered transparently inside `/api/proxy`.

### Logout

`destroyUserSession` deletes the Redis key and clears the cookie.

### Cookie Stores

| Cookie | Expiry | Purpose |
|---|---|---|
| `__user_session` | 7 days | User identity UUID (signs with `USER_SESSION_SECRET` / `SESSION_SECRET`) |
| `__auth_session` | 10 min | Short-lived OAuth flow cookie |
| `_auth` | Session | `remix-auth` internal cookie |

All cookies are httpOnly. Access tokens are never exposed to the browser.

---

## Client-Side Proxy

`GET /api/proxy?path=<backendPath>&<params>` — used by all `react-query` client-side fetches:

1. Reads the `__user_session` cookie.
2. Retrieves (and refreshes if needed) the access token from Redis.
3. Forwards the request to the backend with the `Authorization: Bearer` header.
4. Returns the JSON response.

In production, enforces HTTPS for the backend URL.

---

## State Management

No global React state store. Two mechanisms:

- **Loader data**: `useLoaderData()` for all server-supplied initial data in route components.
- **`react-query` v3**: `usePaginatedQuery` (`app/components/stores/paginatedQuery.tsx`) wraps `useInfiniteQuery` or `useQuery` for client-side paginated/live data. All calls route through `/api/proxy`.

---

## UI Layer

- **shadcn/ui** components in `app/components/ui/shadcn/` — Radix UI wrappers styled with `cn()`.
- **Feature components** organized by domain under `app/components/ui/`: `CreditCards/`, `Dashboard/`, `Subscriptions/`, `Incomes/`, `Investments/`, `Funds/`, `CurrencyExchangeRates/`, `Navigation/`.
- **Icons**: `lucide-react`, `@heroicons/react`, FontAwesome (`@fortawesome/*`).
- **Dark mode**: `next-themes`, theme persisted in `localStorage`, injected via inline `<script>` in `<head>` to prevent flash.
- **Toasts**: `sonner`.
- **Data tables**: `@tanstack/react-table`.
- **Charts**: `recharts`.
- **Split panels**: `react-resizable-panels`.

---

## SSR Specifics

- `entry.server.tsx` — initialises server OTel, renders via `renderToPipeableStream`, logs every request with pino.
- `entry.client.tsx` — initialises browser OTel, hydrates via `hydrateRoot` + `HydratedRouter`.
- All files suffixed `.server.ts` are server-only. `react-router.config.ts` (`serverModuleFormat: "esm"`) prevents these modules (Redis, ioredis, crypto, jose) from being included in the client bundle.
- Route components are isomorphic but receive only serialised loader data — no raw server objects cross the boundary.

---

## Environment Variables

| Variable | Purpose |
|---|---|
| `API_URL` | Backend base URL |
| `ALLOW_INSECURE_API` | Allow `http://` API URL in non-prod |
| `AUTH0_DOMAIN` | Auth0 tenant domain |
| `AUTH0_CLIENT_ID` | OAuth client ID |
| `AUTH0_CLIENT_SECRET` | OAuth client secret |
| `AUTH0_AUDIENCE` | API audience for access tokens |
| `AUTH0_SCOPES` | Comma-separated OAuth scopes |
| `BASE_URL` | App base URL (callback + post-logout redirect) |
| `PORT` | App port |
| `REDIS_URL` | Redis connection string; absent → in-memory mock |
| `USER_SESSION_SECRET` / `SESSION_SECRET` | Cookie signing secrets (comma-separated for rotation) |
| `OTEL_ENABLED` | Toggle OpenTelemetry |
| `OTEL_OTLP_HTTP_ENDPOINT` | OTLP collector URL |
| `NODE_ENV` | `production` enforces HTTPS API URL and secure cookies |

Local dev values live in `FinanceFrontEnd/FinanceApp/.env.development` (gitignored).

---

## Key Patterns and Conventions

| Pattern | Detail |
|---|---|
| `requireAuth` guard | Every protected loader calls `requireAuth(request)` explicitly — no route-level middleware |
| Server-side token exclusivity | Access tokens stay in Redis; browsers never receive them. `/api/proxy` is the only client-side escape hatch |
| `BackendClient` facade | One `BackendClient` per request; all backend calls go through it |
| `BackendUrl` dual-mode | `.toRaw()` for server axios, `.toString()` for proxy URLs — same object, two contexts |
| `.server.ts` naming | Enforced by framework config; prevents server modules leaking into the client bundle |
| Flat route naming | Dots = nested segments, `_index` = index route, `$` = dynamic segment, layout files have no underscore suffix |
| Parallel loaders | `Promise.all([…])` throughout to avoid waterfall backend calls |
| Logging | `pino` with `SafeLogger` wrapper; request logging in `entry.server.tsx` via `pino-http` |
| OpenTelemetry | Dual init — `tracing.server.ts` and `tracing.client.ts` imported at the top of their respective entry points |
