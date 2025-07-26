namespace CQRSDispatch.Interfaces;

/// <summary>
/// Marker interface for query objects in the CQRS pattern.
/// Queries represent operations that retrieve data without modifying system state.
/// </summary>
/// <typeparam name="TResult">The type of data that this query returns.</typeparam>
public interface IQuery<TResult>
{
}

/// <summary>
/// Interface for context-aware queries that support execution context injection.
/// </summary>
/// <typeparam name="TContext">The type of dispatch context.</typeparam>
/// <typeparam name="TResult">The type of data that this query returns.</typeparam>
public interface IContextAwareQuery<TContext, TResult> : IQuery<TResult>, IContextAware<TContext>
    where TContext : DispatchContext
{
}