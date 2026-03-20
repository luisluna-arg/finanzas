# Local Dev Environment

## Running modes

Both frontend and backend can run either way:
- **Locally** (terminal): `dotnet run` / `npm run dev`
- **In Docker**: via docker-compose stacks

## Ports
- Backend: `5000`
- Frontend (`FinanceApp`): `5100`
- Frontend (`FinanceFunds`): `5200`

## Docker stacks
- Infra (Postgres, Redis, Jaeger, Grafana) + Backend: `.infra/local/shared/docker-compose.yaml`
- FinanceApp frontend container: `.infra/local/finances/docker-compose.yaml`
- FinanceFunds frontend container: `.infra/local/funds/docker-compose.yaml`
- Start script: `.bin/powershell/start-finances-local.ps1`

## Env files
- Frontend local dev: `FinanceFrontEnd/FinanceApp/.env.development`
- Frontend Docker: `.infra/local/finances/.env` — uses `host.docker.internal:5000` for `API_URL`
- Backend Docker: `.infra/local/shared/.env`

## Key gotcha: localhost resolves to IPv6 on this machine

`localhost` resolves to `::1` (IPv6), but Docker port bindings only reliably respond on `127.0.0.1` (IPv4).

- When running the frontend locally (`npm run dev`) and the backend in Docker, SSR server-side fetches to `localhost:5000` will **hang/timeout**.
- Fix: use `API_URL=http://127.0.0.1:5000` in `.env.development`.
- Browser-side calls use `window.location.hostname` and are unaffected.
- When both run in Docker this is not an issue — the frontend container uses `host.docker.internal:5000`.
