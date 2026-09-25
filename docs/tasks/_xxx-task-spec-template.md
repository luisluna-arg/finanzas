<!--
Usage: copy this file to `docs/tasks/00N-feature-slug.md` (next available number),
replace every <placeholder> and remove this comment block. Keep the section
structure and the standing Restrictions/Process boilerplate unless the task
genuinely doesn't need them — most tasks in this repo do.
-->

# Task: <Feature name>

## Context & Objective

<One or two sentences: what this feature enables, for whom, and why.>

## Restrictions

- This task is code only, do not attempt to access any environment for testing
- Only unit tests are allowed to be written, for backend and/or frontend — do not run them
- This task will only change <list the specific projects/paths in scope>
- <List any projects/paths explicitly excluded from this work>
- The as final step the new feature should be documented under `\docs\architecture`
- All code must go through review before anything else is run — only make code changes (edit/create files) and compilation checks (e.g. `tsc`/`dotnet build`) to verify the code builds; do not run tests, migrations, or any other command

## Requirements

<!--
Backend and Frontend can each be split into #### subsections by concern (a new
entity, a relationship, an endpoint, a UI area) when the feature is complex
enough to warrant it — don't force subsections on a small feature. Add a
##### Example subsection with a concrete payload/sample wherever an abstract
rule benefits from one — it resolves ambiguity a bare rule leaves open. Be
explicit about things that are easy to leave implicit: ownership/permission
model, database-level invariants vs. application-level ones, soft-delete /
lifecycle behavior, and which side (frontend vs. backend) owns which piece of
logic.
-->

### Backend

<Describe the backend changes: entities, relationships, endpoints, rules.>

### Frontend

<Describe the frontend changes: routes, UI elements, and how they call the backend.>

## Acceptance Criteria

<!--
Mirror the Requirements structure above, but as checkboxes. Backend and
Frontend can each be split into subsections when the feature is complex
enough to warrant it — order those subsections by build/dependency order
(foundational entities and data migrations before the rules that depend on
them, before endpoints). Each item should be independently verifiable by a
reviewer, and should state the resolved rule plainly (not just "TODO: resolve
X") — the level of clarity a human author finds obvious may not be obvious to
whoever implements this from the doc alone.
-->

### Backend

- [ ] <Each backend requirement as its own independently checkable item>

### Frontend

- [ ] <Each UI requirement as its own checkable item>

### Process

- [ ] An implementation plan is drafted as first step, the agent should iterate until there are no inconsistencies. This plan should then be stored under `docs\tasks`
- [ ] The feature is documented under `\docs\architecture` as a final step
- [ ] Only unit tests are added (backend and/or frontend) — no other test types
- [ ] No changes are made outside the projects listed in Restrictions
- [ ] No environment access is attempted for testing (code-only task)
