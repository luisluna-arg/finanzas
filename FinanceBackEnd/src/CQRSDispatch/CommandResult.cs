namespace CQRSDispatch;

/// <summary>
/// Represents the result of a command execution operation.
/// </summary>
/// <param name="isSuccess">Indicates whether the command execution was successful.</param>
/// <param name="errorMessage">Optional error message if the command execution failed.</param>
public class CommandResult : RequestResult
{
    public CommandResult()
        : base(true)
    {
    }

    public CommandResult(bool isSuccess, string? errorMessage = null)
        : base(isSuccess, errorMessage)
    {
    }

    /// <summary>
    /// Creates a successful command result instance.
    /// </summary>
    /// <returns>A new CommandResult instance indicating success.</returns>
    public static CommandResult Success() => Success<CommandResult>();

    /// <summary>
    /// Creates a failed command result instance with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A new CommandResult instance indicating failure with the specified error message.</returns>
    public static CommandResult Failure(string errorMessage) => Failure<CommandResult>(errorMessage);
}
