namespace FinanceApi.Commons;

public class OkResponse
{
    public OkResponse(string message, bool isSuccess = true)
    {
        this.Message = message;
        this.Success = isSuccess;
    }

    public string Message { get; set; }
    public bool Success { get; set; }
}