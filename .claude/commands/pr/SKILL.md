---
name: pr
description: "Handles the full Git Flow lifecycle: updates main, branches, and commits changes in logical batches."
argument-hint: "<short slug>  — short description of the feature or fix (e.g., 'user-auth-api')"
---

# Git Flow & PR Management Skill

**Description:** $ARGUMENTS

## Workflow Steps

1. **Detect Starting Point:**
   * Ensure GH CLI is installed and authenticated.
   * Check the current branch.
   * **If on `main`:** proceed with the full flow (steps 2–5).
   * **If on a non-main branch:** run `gh pr view` to check whether a PR already exists for this branch.
     * If a PR exists: alert the user with the PR URL and stop.
     * If no PR exists: skip steps 2–3, go directly to step 4 (commits) then step 5 (open PR).

2. **Sync Base** _(main-start only)_**:**
   * Execute `git pull origin main` to ensure the local environment is up to date.
   * If there are merge conflicts, stop and alert the user.

3. **Branching** _(main-start only)_**:**
   * Create a new branch using Git Flow naming conventions: `feature/[arg]`, `fix/[arg]`, `refactor/[arg]`, or `docs/[arg]`.
   * Switch to the new branch immediately.

4. **Atomic Commits:**
   * Analyze staged/unstaged changes.
   * Verify no ignored files are accidentally staged (`git status` should not show files that belong in `.gitignore`).
   * Group related file changes into "logical batches" (e.g., all database migrations together, then all service logic). Treat `.gitignore` additions or updates as their own commit (repo config layer).
   * Commit each batch with a concise, imperative message (e.g., "Add user schema", "Implement auth controller").

5. **Open Pull Request:**
   * Use GH CLI: `gh pr create --title "<title>" --body-file <temp-file>`.
   * PR title must follow Conventional Commits: `type(scope): short description (issue #N)`.
   * PR body must follow the template in `.github/PULL_REQUEST_TEMPLATE.md`:
     * **Purpose** — one paragraph explaining _what_ and _why_.
     * **Changes** — grouped by layer/category (e.g., `Repo Config`, `Infrastructure`, `Client Scaffold`, `Docs`). Each group is a `###` heading with bullet points.
     * **Verification** — numbered steps a reviewer can follow to manually verify the change works.
   * Write the body to a temp file and pass via `--body-file` to avoid shell escaping issues (backticks, special chars).

## Constraints

* **Branch Guard:** If on a non-main branch and a PR already exists, alert the user with the PR URL and stop.
* **Safety Check:** If there are merge conflicts during `git pull`, stop and alert the user.
* **Batching Intelligence:** Do not commit all files at once if they touch different layers of the stack (e.g., keep `.css` changes separate from `.cs` or `.js` backend logic). Treat `.gitignore` additions or updates as their own commit (repo config layer).
* **No Preamble:** Execute commands directly and report status only.
