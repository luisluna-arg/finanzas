namespace Finance.Api.Responses;

public class ErrorResponse
{
    public ErrorResponse()
    {
        Title = string.Empty;
        Detail = string.Empty;
        Status = 0;
    }

    public string Title { get; private set; }
    public string Detail { get; private set; }
    public int Status { get; private set; }

    public static ErrorResponse CreateBadRequest(string message)
    {
        return new ErrorResponse
        {
            Title = "Bad request",
            Detail = message,
            Status = StatusCodes.Status500InternalServerError,
        };
    }
}
