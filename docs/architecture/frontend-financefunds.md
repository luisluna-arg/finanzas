# Frontend Architecture — FinanceFunds

## Overview

FinanceFunds is a client-side SPA built with React 19 + Vite. It is a smaller, focused application covering the Funds and Currency Exchange Rates modules. Authentication is handled client-side via the Auth0 React SDK. There is no SSR, no server session, and no proxy — tokens are managed entirely by the Auth0 SDK and injected per-request.

Port: `5200`.

---

## Tech Stack

| Concern | Choice |
|---|---|
| Framework | React 19 |
| Language | TypeScript 5.1 |
| Build tool | Vite 7 (SWC plugin) |
| Rendering | SPA (client-side only) |
| Routing | React Router DOM v7 |
| UI library | Mantine v8 |
| Icons | Tabler Icons |
| Auth | `@auth0/auth0-react` |
| HTTP client | Native `fetch` (no axios) |
| State | Local `useState`/`useEffect` (no global store) |
| Date handling | `dayjs` + `@mantine/dates` |

No react-query, no Zustand, no Redux.

---

## Project Structure

```
src/
  main.tsx                      # Entry — startup guard + provider tree
  App.tsx                       # Root component — router + inline route definitions
  auth/
    auth0-config.ts             # Auth0 domain/clientId from env vars
    AuthProvider.tsx            # Wraps Auth0Provider with config validation
    ProtectedRoute.tsx          # Redirects to Auth0 login if unauthenticated
    useAuth.ts                  # Thin wrapper around useAuth0()
  components/
    Navigation.tsx              # Header: nav links + auth buttons + mobile drawer
    LoginButton.tsx
    LogoutButton.tsx
    ThemeToggle.tsx
    CreateFundModal.tsx
    CreateExchangeRateModal.tsx
    Auth0Debug.tsx              # Dev-only debug panel
    ConfigError.tsx             # Rendered if startup config check fails
  pages/
    FundsDashboard.tsx
    CurrencyExchangeDashboard.tsx
  services/
    ApiClient.ts                # Base fetch client with auth header injection
    FundService.ts
    BankService.ts
    CurrencyService.ts
    CurrencyExchangeRateService.ts
    types/
      FundTypes.ts
      BankTypes.ts
      CurrencyTypes.ts
      CurrencyExchangeRateTypes.ts
  context/
    ThemeContext.tsx             # light/dark state, localStorage persistence
    ThemeContextInstance.ts
    MantineThemeProvider.tsx    # Binds ThemeContext → MantineProvider
    useTheme.ts
  constants/
    currencies.ts               # USD/ARS GUID constants (env-overridable)
  startup/
    checkApiConfig.ts           # Validates VITE_API_URL at startup
  utils/
    SafeLogger.ts               # Dev-only console wrapper; redacts tokens/secrets
    Logger.ts                   # Re-exports SafeLogger
```

---

## Routing

All routes are defined inline in `App.tsx` using React Router DOM `<Routes>`. No file-based routing.

| Path | Component | Guard |
|---|---|---|
| `/` | Redirect → `/funds` | `ProtectedRoute` |
| `/funds` | `FundsDashboard` | `ProtectedRoute` |
| `/exchange-rates` | `CurrencyExchangeDashboard` | `ProtectedRoute` |
| `*` | Redirect → `/funds` | — |

The layout shell is Mantine `AppShell` with a fixed 60 px header rendering `Navigation`. Route content renders inside `AppShell.Main`.

---

## Data Loading

No react-query or SWR. Fetching is fully manual with `useEffect` + `useState` inside each page component.

- `FundsDashboard` — calls `FundService.getAllFunds()` on mount via `useCallback`/`useEffect`.
- `CurrencyExchangeDashboard` — calls `CurrencyExchangeRateService.getLatestExchangeRates()` on mount.
- State updates after fetch are deferred with `setTimeout(..., 0)` to yield back to the browser.

### Service-Level Cache

Each service maintains its own TTL-based in-memory cache (module-level or static class `Map` + timestamps). No external cache library.

| Service | TTL |
|---|---|
| `FundService` | 60 s |
| `BankService` | 10 min |
| `CurrencyService` | 15 min |
| `CurrencyExchangeRateService` | 5 min |

On API error, `BankService` and `CurrencyExchangeRateService` return stale cached data rather than throwing.

---

## Authentication

**Provider:** Auth0 via `@auth0/auth0-react`.

### Flow

1. `main.tsx` runs `getApiConfigError()` before mounting. If validaton fails, `ConfigError` is rendered and the app stops.
2. `AuthProvider` validates that `VITE_AUTH0_DOMAIN` and `VITE_AUTH0_CLIENT_ID` are present, then renders `<Auth0Provider>`.
3. `ProtectedRoute` checks `isAuthenticated`. If not, calls `loginWithRedirect()` with `appState.returnTo` set to the current path.
4. After login, `onRedirectCallback` in `AuthProvider` uses `window.history.replaceState` to restore the original path.

### Token Management

Token storage is handled entirely by the Auth0 SDK (in-memory with silent refresh via hidden iframe). The app never stores tokens manually.

**Token injection into API calls:** `App.tsx` calls `setTokenProvider(getAccessTokenSilently)` once in a `useEffect` on mount. `ApiClient.ts` stores this function reference in module scope (`tokenProvider`). Every request calls `tokenProvider()` immediately before dispatch to get a fresh Bearer token.

---

## API Communication

**Base client:** `src/services/ApiClient.ts` — a hand-rolled class using native `fetch`.

### Base URL Resolution

1. Reads `VITE_API_URL` from env.
2. In dev mode, falls back to `http://localhost:5000` if unset.
3. In non-dev, missing `VITE_API_URL` throws at module load time.
4. Enforces HTTPS in non-dev unless `VITE_ALLOW_INSECURE_API=true` or the URL is localhost.

### Auth Header Injection

`getAuthHeaders()` calls the stored `tokenProvider()` and returns `{ Authorization: 'Bearer <token>', 'Content-Type': 'application/json' }`. Every request goes through this function.

### Endpoints Consumed

| Method | Endpoint |
|---|---|
| `GET` | `/api/summary/currentFunds` |
| `GET` | `/api/funds/:id` |
| `POST` | `/api/funds` |
| `GET` | `/api/banks` |
| `GET` | `/api/currencies` |
| `GET` | `/api/currencies/exchange-rates` |
| `POST` | `/api/currencies/exchange-rates` |
| `PUT` | `/api/currencies/exchange-rates/:id` |

### Service Styles

Two styles coexist: `FundService` is a plain object literal export; `BankService`, `CurrencyService`, and `CurrencyExchangeRateService` are classes with static cache fields exported as default `new XxxService()` instances.

---

## State Management

No global state library. All state is local `useState`:

- Each page owns its own data, loading, and error state.
- Theme (`light`/`dark`) is the only cross-cutting state; it lives in `ThemeContext`, persisted to `localStorage`.

---

## UI Layer

**Component library:** Mantine v8 exclusively. No shadcn, no Radix, no Tailwind in this app (Tailwind config files at the repo root are monorepo artifacts, not used here).

**Theme:** Defined in `MantineThemeProvider.tsx` — custom blue palette, Inter font, `defaultRadius: 'sm'`. Color scheme is driven by `ThemeContext` and bridged into Mantine via `forceColorScheme`.

**Key components:**
- `Navigation` — Mantine `AppShell.Header` with a `Drawer` for mobile menu.
- `CreateFundModal` — Mantine `Modal` + `@mantine/form` + `@mantine/dates` date picker + `@mantine/notifications`.
- `CreateExchangeRateModal` — same pattern for exchange rates.
- `Auth0Debug` — dev-only panel, rendered only when `import.meta.env.MODE === 'development'`.

**Icons:** Tabler Icons (`@tabler/icons-react`).

---

## Environment Variables

| Variable | Required | Purpose |
|---|---|---|
| `VITE_AUTH0_DOMAIN` | Yes | Auth0 tenant domain |
| `VITE_AUTH0_CLIENT_ID` | Yes | Auth0 SPA client ID |
| `VITE_AUTH0_REDIRECT_URI` | No | Callback URL (defaults to `window.location.origin`) |
| `VITE_AUTH0_AUDIENCE` | No | API identifier for access token audience |
| `VITE_API_URL` | No in dev, Yes in prod | Backend base URL |
| `VITE_ALLOW_INSECURE_API` | No | Bypass HTTPS enforcement |
| `VITE_USD_CURRENCY_ID` | No | Override USD GUID constant |
| `VITE_ARS_CURRENCY_ID` | No | Override ARS GUID constant |
| `PORT` | No | Dev server port (default: `5200`) |

`.env` and `.env.development` are present (committed). `.env.example` provides a template.

---

## Key Patterns and Conventions

| Pattern | Detail |
|---|---|
| `@/` path alias | Resolves to `src/` in `vite.config.ts`; used consistently throughout |
| Startup guard | `main.tsx` calls `getApiConfigError()` before mounting; renders `ConfigError` on failure |
| Token provider pattern | `setTokenProvider(getAccessTokenSilently)` decouples the Auth0 SDK from `ApiClient`; the HTTP layer never imports auth directly |
| Manual TTL cache | Each service manages its own `Map` + timestamps; no external library |
| `useMemo`/`useCallback` in pages | Memoized derived values (totals, formatters) and fetch callbacks to avoid unnecessary re-renders |
| `setTimeout(..., 0)` after fetch | Defers state updates to yield back to the browser after async resolution |
| No production logging | `SafeLogger` gates all output to `import.meta.env.DEV`; also redacts keys containing `token`, `secret`, `password`, or `access` |
| Lint enforcement | ESLint flat config + Prettier + Husky pre-commit via `lint-staged`; `no-console` rule bans direct `console.*` calls |
