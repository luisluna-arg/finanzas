namespace FinanceApi.Commons;

public class OkResponse
{
    public OkResponse(string message, bool isSuccess = true)
    {
        Message = message;
        Success = isSuccess;
    }

    public string Message { get; set; }
    public bool Success { get; set; }
}
