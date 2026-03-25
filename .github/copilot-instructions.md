# Project: Finanzas

## Stack
- **Backend**: .NET API — runs in Docker (`finances-shared-backend-1`), port 5000
- **Frontend**: React Router v7 SSR (`FinanceFrontEnd/FinanceApp`) — port 5100
- **Frontend (Funds)**: Vite SPA (`FinanceFrontEnd/FinanceFunds`) — port 5200
- **Infra**: Postgres, Redis, Jaeger, Grafana in Docker via `.infra/local/shared/docker-compose.yaml`

## Local dev
- Frontend and backend can each run locally (terminal) or in Docker
- Env file for local dev: `FinanceFrontEnd/FinanceApp/.env.development`
- Known gotcha: `localhost` resolves to IPv6 on this machine — use `127.0.0.1` for `API_URL` when backend runs in Docker. Details in `.github/docs/local-dev-environment.md`

## Docs (load when relevant)
- `.github/docs/local-dev-environment.md` — run modes, IPv6 gotcha, env files, start script
- `.github/docs/finance-query-filters.md` — EF ownership filters, test identity seeding
- `.github/docs/cqrs-pattern.md` — ICommand/IQuery interfaces, dispatcher methods, base classes, file/folder conventions
- `.github/docs/unit-testing.md` — xUnit test conventions, base classes, EF query filter seeding, dispatcher mocking, known gotchas
