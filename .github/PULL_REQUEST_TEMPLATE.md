# Pull Request Description (use this as a guideline)

Please provide a clear, self-contained description of the change. PR content should follow the structure and level of detail used in the project example (the branch description attached to this PR). The goal is to make reviews fast and deployments safe, especially for schema or migration changes.

What to include (short checklist):
- **Summary**: concise summary of what changed and why. Reference issues with `Fixes #NNN` when applicable.
- **Dependencies**: runtime or deployment prerequisites (DB, external services, CLI tools, etc.).
- **Technical changes**: concrete, per-layer changes (Backend / Frontend / Docs / Others).
- **Migration & Deployment notes**: data-migration steps, backups, breaking changes, rollout guidance.
- **Test scenarios**: minimal manual/automated scenarios reviewers can use to validate behavior.
- **Annotations**: any extra context, links to diagrams, scripts, or external systems.

Use the sections below as a copy-paste starting point. Replace the example content with details for your specific PR.

# Description

Please include a summary of the changes and the related issue. Include the motivation and higher-level context. List any required dependencies for running or deploying this change.

Fixes # (issue)

# Dependencies
- Any requirements for testing or running the project from now on 

# Technical changes

## Backend
- Describe specific backend changes (projects/files/DB tables/commands/queries/services updated)

## Frontend
- Describe frontend changes, if any

## Documentation
- Describe docs added/updated

## Others
- Other changes (scripts, infra, CI, etc.)

# Migration & Deployment Notes
- Back up the database before applying migrations.
- Apply EF migrations prior to routing traffic to the new version.
- Note any rollback steps or manual verification queries.

# Test scenarios

## Backend

```gherkin
Scenario: <Short description>
  Given <initial context>
  When <action>
  Then <expected result>
```

## Frontend

```gherkin
Scenario: <Short description>
  Given <initial context>
  When <action>
  Then <expected result>
```

# Annotations
- Breaking change: schema and data migration required. Back up the DB before applying the migration.
- Deployment: apply EF migrations before routing traffic to this version.
- Verify authorization flows and external scripts that referenced dropped tables.
