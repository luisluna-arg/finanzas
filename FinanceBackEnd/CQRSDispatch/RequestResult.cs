namespace CQRSDispatch;

/// <summary>
/// Abstract base class representing the result of a request operation.
/// Provides common properties and methods for indicating success or failure of operations.
/// </summary>
public abstract class RequestResult
{
    /// <summary>
    /// Initializes a new instance of RequestResult with default values.
    /// </summary>
    public RequestResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of RequestResult with specified success status and error message.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="errorMessage">Optional error message if the operation failed.</param>
    public RequestResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a successful result instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of RequestResult to create.</typeparam>
    /// <returns>A new instance indicating success.</returns>
    protected static T Success<T>()
        where T : RequestResult, new()
        => new T { IsSuccess = true };

    /// <summary>
    /// Creates a failed result instance of the specified type with an error message.
    /// </summary>
    /// <typeparam name="T">The type of RequestResult to create.</typeparam>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A new instance indicating failure with the specified error message.</returns>
    protected static T Failure<T>(string errorMessage)
        where T : RequestResult, new()
        => new T { IsSuccess = false, ErrorMessage = errorMessage };
}
