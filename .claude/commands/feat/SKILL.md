---
name: feat
description: "Create a GitHub feature issue, branch, commit, and publish a feature branch for staged or unstaged changes under one or more given paths, if no given paths then all the unstaged files should be used. Use when: shipping a new feature, committing changes, publishing a feature branch."
argument-hint: "<path1> [path2 ...]  — one or more workspace-relative paths whose changes to include"
---

# GitHub Feature PR Workflow

End-to-end workflow: inspect changes under the given path(s) or the whole workspace if no paths are provided, file a feature issue, create a feature branch, commit only those changes, push, and open a PR populated from the repo's PR template.

## When to Use

- Adding a new feature that touches one or more specific folders or files
- Generating a GitHub issue + PR pair in one step
- Any time the user says "create a feature PR for changes under <path>"
- Works for any language or framework — backend, frontend, infrastructure, scripts, etc.

## Required Inputs

| Input | Example |
|---|---|
| One or more repo-relative paths | `src/api/` `app/components/` `infra/` |

Multiple paths can be provided space-separated or as a list.

## Procedure

### 1 — Inspect changes

**Paths:** $ARGUMENTS

For each provided path run:

```powershell
git diff --stat HEAD -- <path>
git diff HEAD -- <path>
```

Also run `git status` to see any untracked files under the paths.

Use the diffs to understand:
- Which files changed or were added
- What the feature does (new endpoint, new UI component, new service, etc.)
- A concise one-line summary and a fuller description for the issue body

### 2 — Create the GitHub feature issue

Use the GH CLI. Derive the title and body from the diff analysis:

```powershell
gh issue create `
  --title "<concise title describing the feature>" `
  --body "<markdown body: Description / Affected files / Acceptance criteria>" `
  --label enhancement
```

Note the issue number returned (e.g. `#81`).

### 3 — Create a feature branch

Branch name convention: `feature/<short-slug>` derived from the issue title (lowercase, hyphens only).

```powershell
git checkout -b feature/<short-slug>
```

### 4 — Commit only the target paths

Stage exclusively the files under the provided path(s):

```powershell
git add <path1> [<path2> ...]
git commit -m "feat: <one-line summary>

<Optional body paragraph>

Closes #<issue-number>"
```

Do NOT stage files outside the paths the user specified.

### 5 — Run lint before pushing

If any of the changed files are frontend (TypeScript/TSX/JS under `FinanceFrontEnd/`), run lint from the relevant app directory and fix any errors before proceeding. Do **not** push if lint fails.

```powershell
# For FinanceApp changes:
cd FinanceFrontEnd/FinanceApp; npm run lint

# For FinanceFunds changes:
cd FinanceFrontEnd/FinanceFunds; npm run lint
```

If `lint:fix` is available and there are auto-fixable errors, run it and amend the commit:

```powershell
npm run lint:fix
git add <affected-files>
git commit --amend --no-edit
```

### 6 — Publish the branch

```powershell
git push -u origin feature/<short-slug>
```

## Output

Report to the user:
- Issue URL
- Branch name
- Commit SHA (from `git log -1 --oneline`)

Run `/pr` to open the pull request when ready.
