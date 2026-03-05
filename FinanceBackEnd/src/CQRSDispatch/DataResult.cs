namespace CQRSDispatch;

/// <summary>
/// Represents the result of a data operation with typed data.
/// Extends RequestResult to include data payload for successful operations.
/// </summary>
/// <typeparam name="TData">The type of data returned by the operation.</typeparam>
public class DataResult<TData> : RequestResult
{
    /// <summary>
    /// Gets the data returned by the operation.
    /// </summary>
    public TData Data { get; private set; }

    /// <summary>
    /// Initializes a new instance of DataResult with specified parameters.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation execution was successful.</param>
    /// <param name="data">The data returned by the operation.</param>
    /// <param name="errorMessage">Optional error message if the operation execution failed.</param>
    public DataResult(bool isSuccess, TData data, string? errorMessage = null)
        : base(isSuccess, errorMessage)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of DataResult with default values for failure scenarios.
    /// </summary>
    public DataResult() : base(false, null)
    {
        Data = default!;
    }

    /// <summary>
    /// Creates a successful DataResult with the specified data.
    /// </summary>
    /// <param name="data">The data to include in the successful result.</param>
    /// <returns>A DataResult indicating success with the provided data.</returns>
    public static DataResult<TData> Success(TData data)
    {
        var result = Success<DataResult<TData>>();
        result.Data = data;
        return result;
    }

    /// <summary>
    /// Creates a successful DataResult without data.
    /// </summary>
    /// <returns>A DataResult indicating success with default data.</returns>
    public static DataResult<TData> Success()
    {
        var result = Success<DataResult<TData>>();
        return result;
    }

    public static DataResult<TData> Failure(string errorMessage)
    {
        return Failure<DataResult<TData>>(errorMessage);
    }
}
