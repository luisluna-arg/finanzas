namespace Finance.Application.Extensions;

public static class ExceptionExtensions
{
    public static string GetMessage(this Exception ex)
    {
        return ex.InnerException?.Message ?? ex.Message;
    }

    public static string GetStackTrace(this Exception ex)
    {
        return ex.InnerException?.StackTrace ?? ex.StackTrace ?? string.Empty;
    }

    public static string GetFullMessage(this Exception ex)
    {
        return $"{ex.GetType().Name}: {ex.GetMessage()}";
    }

    public static string GetAllMessages(this Exception ex)
    {
        if (ex == null) return string.Empty;
        var messages = new List<string>();
        var current = ex;
        while (current != null)
        {
            messages.Add(current.Message);
            current = current.InnerException;
        }
        return string.Join(" --> ", messages);
    }

    public static string GetInnerMostMessage(this Exception ex)
    {
        if (ex == null) return string.Empty;
        var current = ex;
        while (current.InnerException != null)
        {
            current = current.InnerException;
        }
        return current.Message;
    }
}
