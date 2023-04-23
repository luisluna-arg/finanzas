namespace FinanceApi.Responses;

public class ErrorResponse
{
    public ErrorResponse()
    {
        this.Title = string.Empty;
        this.Detail = string.Empty;
        this.Status = 0;
    }

    public string Title { get; private set; }
    public string Detail { get; private set; }
    public int Status { get; private set; }

    internal static ErrorResponse CreateBadRequest(string message)
    {
        return new ErrorResponse
        {
            Title = "Bad request",
            Detail = message,
            Status = StatusCodes.Status500InternalServerError,
        };
    }
}