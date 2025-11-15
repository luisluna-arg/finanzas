namespace CQRSDispatch.Interfaces;

/// <summary>
/// Marker interface for command objects in the CQRS pattern.
/// Commands represent operations that modify system state.
/// </summary>
public interface ICommand;

/// <summary>
/// Interface for context-aware commands that support execution context injection.
/// </summary>
/// <typeparam name="TContext">The type of dispatch context.</typeparam>
public interface IContextAwareCommand<TContext> : ICommand, IContextAware<TContext>
    where TContext : DispatchContext, new();

/// <summary>
/// Generic interface for command objects that return a specific result type.
/// Inherits from ICommand and constrains the result to RequestResult.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command, must inherit from RequestResult.</typeparam>
public interface ICommand<TResult> : ICommand
    where TResult : RequestResult;

/// <summary>
/// Interface for context-aware commands that return a specific result type and support execution context injection.
/// </summary>
/// <typeparam name="TContext">The type of dispatch context.</typeparam>
/// <typeparam name="TResult">The type of result returned by the command, must inherit from RequestResult.</typeparam>
public interface IContextAwareCommand<TContext, TResult> : ICommand<TResult>, IContextAware<TContext>
    where TContext : DispatchContext, new()
    where TResult : RequestResult;
