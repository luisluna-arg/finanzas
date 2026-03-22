# Backend Feature — .NET API

Full step-by-step guide for adding a new entity to the `FinanceBackEnd` solution.

> **IMPORTANT — Read this entire file before starting.** All steps below are required. Steps 1–8 are implementation; **Step 9 (unit tests) is mandatory** and must not be skipped.

---

## Required Steps (all mandatory)

1. Domain Model
2. Persistence Configuration + Migration
3. DTO
4. Commands (Create, Update, Delete)
5. Queries (GetAll, GetSingle, GetPaginated)
6. Mapper
7. Service Requests
8. API Controllers (Command + Query)
9. **Unit Tests** — command handler tests, query handler tests, service tests

---

## Step 1 — Domain Model

**Location:** `FinanceBackEnd/src/Finance.Domain/Models/<EntityName>s/<EntityName>.cs`

Inherit from:
- `Entity<Guid>` — for simple entities
- `AuditedEntity<Guid>` — for entities that need `CreatedAt` / `UpdatedAt`

```csharp
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models.<EntityName>s;

public class <EntityName> : AuditedEntity<Guid>
{
    public string Name { get; set; } = default!;
    // add properties; use Guid FKs + virtual navigation properties
}
```

---

## Step 2 — Persistence Configuration

### 2a. EF Core configuration

**Location:** `FinanceBackEnd/src/Finance.Persistence/Configurations/<EntityName>Configuration.cs`

```csharp
using Finance.Domain.Models.<EntityName>s;
using Finance.Persistence.Configurations.Base;

namespace Finance.Persistence.Configurations;

public class <EntityName>Configuration : AuditedEntityConfiguration<<EntityName>, Guid>;
```

Use `EntityConfiguration` base if the entity does NOT use auditing.

### 2b. Register DbSet

**File:** `FinanceBackEnd/src/Finance.Persistence/FinanceDbContext.cs`

```csharp
public DbSet<<EntityName>> <EntityName>s { get; set; }
```

### 2c. Create & apply migration

```powershell
cd FinanceBackEnd
dotnet ef migrations add Add<EntityName> --project src/Finance.Migrations --startup-project src/Finance.Api
dotnet ef database update --project src/Finance.Migrations --startup-project src/Finance.Api
```

---

## Step 3 — DTO

**Location:** `FinanceBackEnd/src/Finance.Application/Dtos/<EntityName>s/<EntityName>Dto.cs`

```csharp
using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.<EntityName>s;

public record <EntityName>Dto : Dto<Guid>
{
    // expose properties; use nested DTOs for navigation properties
}
```

---

## Step 4 — Commands (Write Side)

### 4a. Base command + validator

**Location:** `FinanceBackEnd/src/Finance.Application/Commands/<EntityName>s/_Base/Upsert<EntityName>BaseCommand.cs`

```csharp
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Models.<EntityName>s;
using FluentValidation;

namespace Finance.Application.Commands.<EntityName>s.Base;

public abstract class Upsert<EntityName>BaseCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<<EntityName>>>
{
    // shared properties for Create and Update
    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public abstract class Upsert<EntityName>BaseCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : Upsert<EntityName>BaseCommand
{
    protected Upsert<EntityName>BaseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}
```

### 4b. Create command

**Location:** `FinanceBackEnd/src/Finance.Application/Commands/<EntityName>s/Create<EntityName>Command.cs`

```csharp
using CQRSDispatch;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.<EntityName>s.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.<EntityName>s;
using Finance.Persistence;

namespace Finance.Application.Commands.<EntityName>s;

public class Create<EntityName>CommandHandler : BaseCommandHandler<Create<EntityName>Command, <EntityName>>
{
    private readonly IRepository<<EntityName>, Guid> _repository;

    public Create<EntityName>CommandHandler(FinanceDbContext db, IRepository<<EntityName>, Guid> repository) : base(db)
    {
        _repository = repository;
    }

    public override async Task<DataResult<<EntityName>>> ExecuteAsync(Create<EntityName>Command command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new Create<EntityName>CommandValidator());

        var entity = new <EntityName>
        {
            // map command properties
        };

        await _repository.AddAsync(entity, cancellationToken);
        return DataResult<<EntityName>>.Success(entity);
    }
}

public class Create<EntityName>Command : Upsert<EntityName>BaseCommand;
public class Create<EntityName>CommandValidator : Upsert<EntityName>BaseCommandValidator<Create<EntityName>Command>;
```

### 4c. Update command

**Location:** `FinanceBackEnd/src/Finance.Application/Commands/<EntityName>s/Update<EntityName>Command.cs`

Same pattern as Create. Handler loads the existing entity by Id, calls `entity.Update(...)`, saves, and returns it.

### 4d. Delete command

**Location:** `FinanceBackEnd/src/Finance.Application/Commands/<EntityName>s/Delete<EntityName>sCommand.cs`

Inherit from `DeleteEntityCommand` or `DeleteEntityOwnerCommand` if ownership enforcement is needed.

### 4e. Activate / Deactivate (optional)

Inherit from `BaseActivateCommandHandler` / `BaseDeactivateCommandHandler` for soft-delete toggle support.

---

## Step 5 — Queries (Read Side)

**Location:** `FinanceBackEnd/src/Finance.Application/Queries/<EntityName>s/`

| File | Base class | Purpose |
|------|-----------|---------|
| `Get<EntityName>sQuery.cs` | `GetAllQuery<TEntity>` | List with optional filters |
| `GetSingle<EntityName>Query.cs` | `GetSingleByIdQuery<TEntity>` | Single by Id |
| `GetPaginated<EntityName>sQuery.cs` | `GetPaginatedQuery<TEntity>` | Paginated list |

```csharp
using CQRSDispatch;
using Finance.Application.Commands.Base;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Domain.Models.<EntityName>s;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.<EntityName>s;

public class Get<EntityName>sQuery : GetAllQuery<<EntityName>>
{
    // optional filter properties (e.g. public string? Name { get; set; })
}

public class Get<EntityName>sQueryHandler : BaseCollectionQueryHandler<Get<EntityName>sQuery, <EntityName>>
{
    private readonly IRepository<<EntityName>, Guid> _repository;

    public Get<EntityName>sQueryHandler(FinanceDbContext db, IRepository<<EntityName>, Guid> repository) : base(db)
    {
        _repository = repository;
    }

    public override async Task<DataResult<List<<EntityName>>>> ExecuteAsync(Get<EntityName>sQuery request, CancellationToken cancellationToken)
    {
        IQueryable<<EntityName>> query = _repository.GetDbSet()
            // .Include(o => o.RelatedEntity)
            .AsQueryable();

        if (!request.IncludeDeactivated)
            query = query.Where(o => !o.Deactivated);

        // apply additional filters using FilterBy extension where possible

        return DataResult<List<<EntityName>>>.Success(await query.ToListAsync(cancellationToken));
    }
}
```

---

## Step 6 — Mapper

**Location:** `FinanceBackEnd/src/Finance.Application/Mapping/Mappers/<EntityName>Mapper.cs`

```csharp
using Finance.Application.Dtos.<EntityName>s;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.<EntityName>s;

namespace Finance.Application.Mapping.Mappers;

public class <EntityName>Mapper : BaseMapper<<EntityName>, <EntityName>Dto>, I<EntityName>Mapper
{
    public <EntityName>Mapper(IMappingService mappingService) : base(mappingService) { }
}

public interface I<EntityName>Mapper : IMapper<<EntityName>, <EntityName>Dto>;
```

Register the mapping profile in `MappingConfigExtensions.cs` if a custom profile is needed.

---

## Step 7 — Service Requests

**Location:** `FinanceBackEnd/src/Finance.Application/Services/<EntityName>s/<EntityName>Requests.cs`

```csharp
namespace Finance.Application.Services.<EntityName>s;

public sealed record Create<EntityName>Request(/* fields */);
public sealed record Update<EntityName>Request(Guid Id, /* fields */);
public sealed record Delete<EntityName>Request(Guid[] Ids);
// Add SetOwner / DeleteOwner request types if resource ownership is used
```

---

## Step 8 — API Controllers

### 8a. Command controller

**Location:** `FinanceBackEnd/src/Finance.Api/Controllers/Commands/<EntityName>CommandController.cs`

```csharp
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.<EntityName>s;
using Finance.Application.Mapping;
using Finance.Application.Services.<EntityName>s;
using Finance.Domain.Models.<EntityName>s;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/<entity-route>")]
public class <EntityName>CommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    <EntityName>Service service)
    : CommandController<
        <EntityName>,
        <EntityName>Permissions,
        Create<EntityName>Request,
        Update<EntityName>Request,
        Delete<EntityName>Request,
        Set<EntityName>OwnerRequest,
        Delete<EntityName>OwnerRequest,
        Guid,
        <EntityName>Dto,
        <EntityName>Service>(mapper, dispatcher, service)
{
}
```

### 8b. Query controller

**Location:** `FinanceBackEnd/src/Finance.Api/Controllers/Queries/<EntityName>QueryController.cs`

```csharp
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Mapping;
using Finance.Application.Queries.<EntityName>s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/<entity-route>")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class <EntityName>QueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : SecuredApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Get<EntityName>sQuery request)
    {
        var result = await dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingle<EntityName>Query request)
    {
        var result = await dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginated<EntityName>sQuery request)
    {
        var result = await dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }
}
```

### 8c. Build check

```powershell
dotnet build FinanceBackEnd/src/Finance.Api
```

---

## Step 9 — Unit Tests

**Project:** `FinanceBackEnd/tests/Finance.Application.Tests`

Global usings (`Usings.cs`) already declare `Xunit` and `Moq` — no extra imports needed for those.
Use an in-memory `FinanceDbContext` with a unique database name per test class to keep tests isolated.

---

### 9a. Command handler tests

**Location:** `tests/Finance.Application.Tests/Commands/<EntityName>s/Create<EntityName>CommandHandlerTests.cs`

```csharp
using Finance.Application.Commands.<EntityName>s;
using Finance.Application.Repositories;
using Finance.Domain.Models.<EntityName>s;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.<EntityName>s;

public class Create<EntityName>CommandHandlerTests : IDisposable
{
    private readonly Mock<IRepository<<EntityName>, Guid>> _repository;
    private readonly FinanceDbContext _dbContext;

    public Create<EntityName>CommandHandlerTests()
    {
        _repository = new Mock<IRepository<<EntityName>, Guid>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task Create_HappyPath_AddsEntityAndReturnsSuccess()
    {
        var command = new Create<EntityName>Command { /* set required properties */ };

        _repository
            .Setup(r => r.AddAsync(It.IsAny<<EntityName>>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var handler = new Create<EntityName>CommandHandler(_dbContext, _repository.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        _repository.Verify(r => r.AddAsync(It.IsAny<<EntityName>>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Create_WhenRequiredFieldMissing_ThrowsValidationException()
    {
        var command = new Create<EntityName>Command { /* leave required field empty */ };
        var handler = new Create<EntityName>CommandHandler(_dbContext, _repository.Object);

        await Assert.ThrowsAnyAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }
}
```

Create a matching `Update<EntityName>CommandHandlerTests.cs` following the same pattern.

---

### 9b. Query handler tests

**Location:** `tests/Finance.Application.Tests/Queries/<EntityName>s/<EntityName>QueryHandlerTests.cs`

Use a real in-memory `FinanceDbContext` seeded with known data; mock only `IRepository<T>` to return `_dbContext.<EntityName>s` via `GetDbSet()`.

```csharp
using Finance.Application.Queries.<EntityName>s;
using Finance.Application.Repositories;
using Finance.Domain.Models.<EntityName>s;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Queries.<EntityName>s;

public class <EntityName>QueryHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public <EntityName>QueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task Get<EntityName>s_FiltersDeactivated_ReturnsOnlyActive()
    {
        await _dbContext.<EntityName>s.AddRangeAsync(
            new <EntityName> { Id = Guid.NewGuid(), Deactivated = false /* ... */ },
            new <EntityName> { Id = Guid.NewGuid(), Deactivated = true  /* ... */ }
        );
        await _dbContext.SaveChangesAsync();

        var repository = new Mock<IRepository<<EntityName>, Guid>>();
        repository.Setup(r => r.GetDbSet()).Returns(_dbContext.<EntityName>s);

        var handler = new Get<EntityName>sQueryHandler(_dbContext, repository.Object);
        var result = await handler.ExecuteAsync(new Get<EntityName>sQuery { IncludeDeactivated = false }, default);

        Assert.True(result.IsSuccess);
        Assert.All(result.Data, e => Assert.False(e.Deactivated));
    }
}
```

---

### 9c. Service tests (partial class pattern)

The service test class is split into multiple files using `partial class`. Keep the fixture in a base file and each operation in its own file.

**Base fixture** — `tests/Finance.Application.Tests/Services/<EntityName>s/_<EntityName>ServiceTests.cs`

```csharp
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services.<EntityName>s;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Services.<EntityName>s;

public partial class <EntityName>ServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly <EntityName>Service _sut;

    public <EntityName>ServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new <EntityName>Service(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();
}
```

**Per-operation files** — e.g. `<EntityName>ServiceTests.Create.cs`

```csharp
using CQRSDispatch;
using Finance.Application.Commands.<EntityName>s;
using Finance.Application.Services.<EntityName>s;
using Finance.Domain.Models.<EntityName>s;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Tests.Services.<EntityName>s;

public partial class <EntityName>ServiceTests
{
    [Fact]
    public async Task Create_WhenDispatchSucceeds_ReturnsSuccess()
    {
        var entity = new <EntityName> { Id = Guid.NewGuid() };
        var request = new Create<EntityName>Request(/* fields */);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<<EntityName>>>(It.IsAny<Create<EntityName>Command>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<<EntityName>>.Success(entity));

        var result = await _sut.Create(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(entity, result.Data);
    }

    [Fact]
    public async Task Create_WhenDispatchFails_ReturnsFailure()
    {
        var request = new Create<EntityName>Request(/* fields */);

        _dispatcher
            .Setup(d => d.DispatchAsync<DataResult<<EntityName>>>(It.IsAny<Create<EntityName>Command>(), It.IsAny<HttpRequest?>()))
            .ReturnsAsync(DataResult<<EntityName>>.Failure("error message"));

        var result = await _sut.Create(request);

        Assert.False(result.IsSuccess);
    }
}
```

Create equivalent files for `Update`, `Delete`, `Activate`, `Deactivate` as applicable.

### 9d. Run tests

```powershell
dotnet test FinanceBackEnd/tests/Finance.Application.Tests
```

---

## Checklist

- [ ] Domain entity created
- [ ] EF config added + `DbSet` registered on `FinanceDbContext`
- [ ] Migration created and applied
- [ ] DTO created
- [ ] Commands: Create, Update, Delete (+ Activate/Deactivate if needed)
- [ ] Queries: GetAll, GetSingle, GetPaginated
- [ ] Mapper + interface created
- [ ] Service requests file created
- [ ] Command controller added
- [ ] Query controller added
- [ ] `dotnet build` passes
- [ ] Command handler tests added
- [ ] Query handler tests added
- [ ] Service tests added (partial class per operation)
- [ ] `dotnet test` passes
