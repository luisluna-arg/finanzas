---
name: commit
description: "Commit staged or unstaged changes in logical batches without creating branches or PRs. Use when: you have changes ready to commit on the current branch and want them grouped by layer."
argument-hint: "[path1 path2 ...]  — optional paths to restrict which changes are committed; defaults to all unstaged/staged changes"
---

# Commit Changes in Logical Batches

Analyze all pending changes (or those under the given paths) and commit them in logical groups — one commit per layer of the stack. No branch creation, no PR, no pushing.

## Paths

$ARGUMENTS

If no paths are provided, include all staged and unstaged changes visible in `git status`.

## Procedure

### 1 — Inspect changes

Run the following to understand what has changed:

```powershell
git status
git diff --stat HEAD
```

If paths were provided, restrict analysis to those paths:

```powershell
git diff --stat HEAD -- <path1> [<path2> ...]
```

Also note any untracked files that should be included.

### 2 — Safety check

Verify no secrets, `.env` files, or files that belong in `.gitignore` are about to be staged.  
If any are found, warn the user and exclude them.

### 3 — Group into logical batches

Assign each changed file to exactly one batch based on its layer. Use this priority order:

| Batch | What belongs here |
|---|---|
| **Repo / DevEx** | `.claude/`, `.github/`, `.gitignore`, root config files (`.editorconfig`, `*.sln`) |
| **Infrastructure** | `docker-compose*`, `Dockerfile*`, CI workflows, `infra/` |
| **Dependencies** | `*.csproj`, `package.json`, `package-lock.json`, `*.lock` |
| **Domain / Config** | Domain models, enums, config classes, constants |
| **Backend Logic** | Services, handlers, helpers, repositories, commands, queries |
| **API / Controllers** | Controllers, DTOs, mappers, API-level filters |
| **Frontend** | Any file under `FinanceFrontEnd/` |
| **Tests** | Any file under `*.Tests/` or `**/*Tests*` |
| **Docs** | `*.md`, `docs/` |

Files that span multiple concerns should go in the most specific batch (e.g., a controller that also adds a DTO goes in **API / Controllers**).

### 4 — Commit each batch

For each batch (skip empty ones), stage and commit:

```powershell
git add <files-in-batch>
git commit -m "<type>(<scope>): <imperative summary>"
```

Commit message rules:
- Use Conventional Commits: `feat`, `fix`, `refactor`, `chore`, `test`, `docs`
- Scope is the layer or module (e.g., `pdf-import`, `statement-config`, `frontend`)
- Summary is imperative, lowercase, no period, ≤72 chars
- No trailing attributions (`Co-Authored-By`, `Generated with`, etc.)

### 5 — Report

After all commits, run:

```powershell
git log --oneline -10
```

Report each commit SHA and message so the user can see what was created.

## Constraints

- **Never** stage files outside the provided paths (if paths were given).
- **Never** amend existing commits — always create new ones.
- **Never** push — that is the caller's responsibility.
- **Never** skip hooks (`--no-verify`).
- If a commit hook fails, fix the underlying issue and retry.
- **No Preamble:** Execute commands directly and report status only.
