# Pull Request Template

This template helps structure PR descriptions for efficient reviews and safe deployments. Delete the example text and replace with your specific changes.

## Quick Checklist
- [ ] Summary explains what changed and why
- [ ] Dependencies are clearly listed
- [ ] Technical changes are detailed by layer
- [ ] Database/migration steps documented
- [ ] Test scenarios provided
- [ ] Breaking changes highlighted

---

# Summary

Brief description of what changed and why.

**Fixes:** #(issue_number) <!-- if applicable -->

# Dependencies
<!-- List any requirements for testing/deployment -->
- Dependencies here

# Technical Changes

## Backend
- Specific backend changes

## Frontend
- Frontend changes, if any

## Documentation
- Documentation updates

## Infrastructure
- Docker, CI/CD, or configuration changes

# Migration & Deployment
<!-- Critical for schema/data changes -->
- [ ] Back up database before deployment
- [ ] Apply migrations: `describe migration steps`
- [ ] Verify: `specific verification steps`

# Test Scenarios

**Backend:**
```gherkin
Scenario: Feature works correctly
  Given initial state
  When action is performed
  Then expected result occurs
```

**Frontend:**
```gherkin
Scenario: UI responds appropriately
  Given user context
  When user interaction
  Then UI updates as expected
```

# Breaking Changes
<!-- List any breaking changes -->
- None / List breaking changes

# Additional Notes
<!-- Context, diagrams, external links -->
