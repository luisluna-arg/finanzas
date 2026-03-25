# CQRS Pattern

## Result types

```
RequestResult (abstract)
├── IsSuccess: bool
├── ErrorMessage: string?
│
├── CommandResult         — void commands
│     Success() / Failure(msg)
│
└── DataResult<TData>     — data-returning commands & queries
      Data: TData
      Success(data) / Failure(msg)
```

## Interfaces

```csharp
// Commands
interface ICommand;                                      // void → returns CommandResult
interface ICommand<TResult> : ICommand                  // data → returns DataResult<T>

// Handlers
interface ICommandHandler<TCommand, TResult>             // TResult : RequestResult
interface ICommandHandler<TCommand>                      // void variant → returns CommandResult

// Queries
interface IQuery<TResult>;
interface IQueryHandler<TQuery, TResult>                 // always returns DataResult<TResult>
```

## Dispatcher

```csharp
// Commands
Task<TResult>             DispatchAsync<TResult>(ICommand<TResult> command);
Task<CommandResult>       DispatchCommandAsync(ICommand command);

// Queries
Task<DataResult<TResult>> DispatchQueryAsync<TResult>(IQuery<TResult> query);
```

Context-aware variants (`IContextAwareCommand/Query<TContext, ...>`) accept an additional `HttpRequest?` parameter.

## Base classes

```csharp
// Commands (in Finance.Application)
abstract class BaseCommandHandler<TCommand, TEntity>
    : ICommandHandler<TCommand, DataResult<TEntity>>
{
    protected FinanceDbContext DbContext { get; }
    public abstract Task<DataResult<TEntity>> ExecuteAsync(TCommand command, CancellationToken ct);
}

// Queries (in Finance.Application)
abstract class BaseQueryHandler<TQuery, TEntity>
    : IQueryHandler<TQuery, TEntity>
{
    protected FinanceDbContext DbContext { get; }
    public abstract Task<DataResult<TEntity>> ExecuteAsync(TQuery request, CancellationToken ct);
}
```

## Conventions

- Command + handler are **co-located in the same file** under `Finance.Application/Commands/<Domain>/`.
- Query + handler are **co-located in the same file** under `Finance.Application/Queries/<Domain>/`.
- Query base classes live in `Finance.Application/Queries/_Base/`: `GetAllQuery<T>`, `GetPaginatedQuery<T>`, `GetSingleByIdQuery<T, TId>`.
- Commands always return `DataResult<TEntity>` even for write operations (returns the created/updated entity).
- Use `DataResult<T>.Failure(msg)` for expected failures — no exceptions.
