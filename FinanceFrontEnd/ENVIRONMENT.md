# Frontend environment and deployment notes

Required environment variables (production)

-   `VITE_API_URL` — full HTTPS URL to the backend API (e.g. https://api.example.com). This is required in production and validated at startup/CI.

Optional override (use only in controlled staging/test)

-   `VITE_ALLOW_INSECURE_API=true` (client) or `ALLOW_INSECURE_API=true` (server/CI) — allows `http://` API URLs for environments where HTTPS is intentionally not used. Use sparingly and never in public production.

Dockerfile guidance

-   Avoid passing secrets via Docker build ARGs or ENV. The repository validator flags ARGs like `AUTH0_CLIENT_SECRET`, `CLIENT_SECRET`, `PASSWORD`, `TOKEN` as potential secrets. Instead:
    -   Inject secrets at runtime using secrets managers (Kubernetes Secrets, Docker secrets, CI/CD secret variables).
    -   Use build-time ARGs only for non-secret configuration.

CI

-   The repo contains a GitHub Actions workflow `.github/workflows/validate-config.yml` that runs `scripts/validate-config.js`. The script:
    -   Fails if `VITE_API_URL` is missing or uses non-HTTPS in production unless ALLOW_INSECURE_API is set.
    -   Fails if Dockerfiles contain secret-like ARGs.

Example: provide `VITE_API_URL` and optional `ALLOW_INSECURE_API` via GitHub Actions secrets

1. In the repository settings -> Secrets, create `VITE_API_URL` (the HTTPS API URL) and, if needed for staging, `ALLOW_INSECURE_API`.

2. The `validate-config` workflow will inject those secrets into the validation step's environment. Do NOT print them in logs.

3. For builds that require runtime secrets, add the secrets to your deployment or runtime environment (Kubernetes Secrets, Azure Key Vault, or GitHub Actions environments) rather than passing them as build-time ARG.

Using GitHub Environments for staging/preview (recommended)

-   Create an Environment named `staging` in the repository settings.
-   Add environment-level secrets (for example `VITE_API_URL`, `AUTH0_CLIENT_SECRET`) to the `staging` environment.
-   Protect the environment with required reviewers if you want manual approval before workflows can access secrets.
-   The `validate-config.yml` workflow includes a `validate-staging` job that binds to the `staging` environment; only workflows with permission and environment approval can use its secrets.
