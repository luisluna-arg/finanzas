# Finanzas — Claude Code Instructions

See [AGENTS.md](AGENTS.md) for full project context: what the project does, repo layout, tech stack, architecture references, build commands, coding conventions, and code changes protocol.

## Workflow Skills

Project-specific skills live in `.claude/commands/`:

| Skill | When to use |
|---|---|
| `/feature` | Implementing a new full-stack feature (entity → backend → frontend) |
| `/unit-tests` | Writing backend xUnit tests for new or changed .NET code |
| `/bug-fix` | Creating a GitHub bug issue, committing, and pushing the fix branch |
| `/docs` | Creating a GitHub docs issue, committing, and pushing the docs branch |
| `/feat` | Creating a GitHub feature issue, committing, and pushing the feature branch |
| `/pr` | Opening the pull request once a branch is pushed |
| `/infra` | Making infrastructure changes (compose, CI, Dockerfiles) on a branch |
| `/security-review-backend` | Security audit of the .NET backend |
| `/security-review-frontend` | Security audit of the React frontends |
