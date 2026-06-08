---
name: docs
description: "Create a GitHub docs issue, branch, commit, and publish a docs branch for staged or unstaged changes under one or more given paths, if no given paths then all the unstaged files should be used. Use when: shipping documentation changes, committing doc updates, publishing a docs branch."
argument-hint: "<path1> [path2 ...]  — one or more workspace-relative paths whose changes to include"
---

# GitHub Docs PR Workflow

End-to-end workflow: inspect changes under the given path(s) or the whole workspace if no paths are provided, file a docs issue, create a docs branch, commit only those changes, push, and open a PR populated from the repo's PR template.

## When to Use

- Adding or updating documentation (README, guides, architecture docs, skill files, etc.)
- Generating a GitHub issue + PR pair in one step for doc-only changes
- Any time the user says "create a docs PR for changes under <path>"
- Works for any documentation format — Markdown, ADRs, diagrams, comments, etc.

## Required Inputs

| Input | Example |
|---|---|
| One or more repo-relative paths | `.github/docs/` `docs/` `README.md` |

Multiple paths can be provided space-separated or as a list.

## Procedure

### 1 — Inspect changes

For each provided path run:

```powershell
git diff --stat HEAD -- <path>
git diff HEAD -- <path>
```

Also run `git status` to see any untracked files under the paths.

Use the diffs to understand:
- Which files changed or were added
- What the documentation covers (new guide, updated reference, corrected info, etc.)
- A concise one-line summary and a fuller description for the issue body

### 2 — Create the GitHub docs issue

Use the GH CLI. Derive the title and body from the diff analysis:

```powershell
gh issue create `
  --title "<concise title describing the documentation change>" `
  --body "<markdown body: Description / Affected files / What was added or corrected>" `
  --label documentation
```

Note the issue number returned (e.g. `#82`).

### 3 — Create a docs branch

Branch name convention: `docs/<short-slug>` derived from the issue title (lowercase, hyphens only).

```powershell
git checkout -b docs/<short-slug>
```

### 4 — Commit only the target paths

Stage exclusively the files under the provided path(s):

```powershell
git add <path1> [<path2> ...]
git commit -m "docs: <one-line summary>

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
git push -u origin docs/<short-slug>
```

## Output

Report to the user:
- Issue URL
- Branch name
- Commit SHA (from `git log -1 --oneline`)

Run `/pr` to open the pull request when ready.
