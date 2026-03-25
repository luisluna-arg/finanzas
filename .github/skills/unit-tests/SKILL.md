---
name: unit-tests
description: "Write backend unit tests for new or changed .NET code. Use when: adding tests for unstaged changes, writing tests for a specific file or set of files, covering a new handler, service, or domain type. Reads source files, infers test cases, writes xUnit tests following project conventions, then builds and runs to verify."
argument-hint: "[file1 file2 ...]  — workspace-relative paths whose tests to write; omit to use all unstaged changes"
---

# Unit Test Implementation Workflow

Workflow for writing xUnit tests for changed or specified backend source files, following the conventions of `Finance.Application.Tests`.

## Reference Doc

Load the full testing guide before starting:

```
.github/docs/unit-testing.md
```

This doc covers base classes, in-memory DB setup, EF query filter seeding, dispatcher mocking, and known gotchas. **Do not proceed without reading it.**

---

## Step 1 — Identify target files

### If paths are provided

Use the given paths as the target sources directly.

### If no paths are provided

Run:

```powershell
git diff --name-only HEAD
git status --short
```

Filter to `.cs` files under `FinanceBackEnd/src/Finance.Application/` (Commands, Queries, Services). Ignore migration files, generated files, domain models, and EF configuration.

---

## Step 2 — Understand each source file

For each source file:

1. Read it fully.
2. Identify:
   - Class type: command handler, query handler, or service
   - Constructor dependencies (what to mock vs inject)
   - Whether it accesses `FinanceDbContext` directly
   - Which entities it reads/writes and whether those have EF query filters (see `.github/docs/unit-testing.md`)
   - Public methods / `ExecuteAsync` signature and return type
3. Read the existing test file (if any) to avoid duplicating covered cases.

---

## Step 3 — Find the existing test file

Test file location mirrors source location:

| Source path | Test path |
|---|---|
| `src/Finance.Application/Commands/Funds/CreateFundCommandHandler.cs` | `tests/Finance.Application.Tests/Commands/Funds/CreateFundCommandHandlerTests.cs` |
| `src/Finance.Application/Queries/Currencies/GetCurrencyQueryHandler.cs` | `tests/Finance.Application.Tests/Queries/Currencies/GetCurrencyQueryHandlerTests.cs` |
| `src/Finance.Application/Services/CurrencyConversionService.cs` | `tests/Finance.Application.Tests/Services/CurrencyConversionServiceTests.cs` |

If the test file exists, **add tests to it**. If not, create a new one.

---

## Step 4 — Plan test cases

Think through:

- **Happy path(s)**: the handler succeeds under normal conditions
- **Validation / failure paths**: missing entity, invalid input → expected error or false result
- **Boundary conditions**: empty collections, null values, zero amounts
- **Filter/ownership paths**: entity not returned because permission not granted
- **Behavioral assertions**: dispatcher called once vs. not at all (for caching, short-circuits)

Write the list before coding.

---

## Step 5 — Write the tests

### Class skeleton

```csharp
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Commands.<Module>;   // or Queries / Services

public class XxxCommandHandlerTests : QueryHandlerBaseTests
{
    // mocks declared here
    public XxxCommandHandlerTests() { /* set up mocks */ }

    private XxxCommandHandler CreateHandler() => new(_dbContext, _mock.Object, ...);

    [Fact]
    public async Task MethodName_Condition_ExpectedOutcome() { ... }
}
```

### Naming

`MethodOrAction_Condition_ExpectedOutcome` — e.g. `Upload_WhenHelperReturnsNoRecords_ReturnsFailure`.

### Key rules (from `.github/docs/unit-testing.md`)

- Extend `QueryHandlerBaseTests` for **every** test class that touches `FinanceDbContext`.
- Seed `Identity { SourceId = "IdentityNotFound" }` on the current user so EF query filters match.
- Seed `*Permissions` rows for every entity that has an ownership filter or the query will return nothing.
- For `IOLInvestment` tests: grant **both** `IOLInvestmentPermissions` and `IOLInvestmentAssetPermissions`.
- `ConvertCollection` always calls the dispatcher — set up rates even for same-currency input.
- Override `Dispose()` and call `base.Dispose()` first if the class holds an `IMemoryCache`.
- Do not add code comments unless the logic is non-obvious.

### Avoid

- Don't add narrative comments (e.g. `// Arrange`, `// Act`, `// Assert`, `// Set up the handler`).
- Don't test behavior already covered in the existing test file.
- Don't add `using` directives for namespaces not directly referenced in the file.

---

## Step 6 — Build

```powershell
dotnet build FinanceBackEnd/tests/Finance.Application.Tests/Finance.Application.Tests.csproj /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary;ForceNoAlign
```

Fix every compiler error before running tests. Common errors:

| Error | Fix |
|---|---|
| `IgnoreQueryFilters`/`CountAsync` not found | Add `using Microsoft.EntityFrameworkCore;` |
| IDE0005 unused `using` | Remove the import |
| Type not found | Check the correct namespace in the source file |

---

## Step 7 — Run tests

Run only the newly written test classes first:

```powershell
dotnet test FinanceBackEnd/tests/Finance.Application.Tests/ --filter "FullyQualifiedName~XxxTests" -v n
```

Then run the full suite to check for regressions:

```powershell
dotnet test FinanceBackEnd/tests/Finance.Application.Tests/ -v q
```

Interpret failures:

- **Empty collection / no results**: likely missing `*Permissions` seed.
- **NullReferenceException in ConvertCollection**: missing dispatcher setup.
- **Wrong numeric result**: check whether rate direction is buy vs. sell.

Fix failures and re-run until all tests pass.

---

## Step 8 — Done

Report:
- Which test files were created or modified
- How many tests were added and which pass
- Any tests left failing and the known reason
