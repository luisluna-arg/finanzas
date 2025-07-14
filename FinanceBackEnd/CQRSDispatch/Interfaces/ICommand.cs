namespace CQRSDispatch.Interfaces;

/// <summary>
/// Marker interface for command objects in the CQRS pattern.
/// Commands represent operations that modify system state.
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Generic interface for command objects that return a specific result type.
/// Inherits from <see cref="ICommand"/> and constrains the result to <see cref="RequestResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command, must inherit from <see cref="RequestResult"/>.</typeparam>
public interface ICommand<TResult> : ICommand
    where TResult : RequestResult
{
}