namespace FinanceApi.Helpers;

public static class StringHelper
{
    public static string? CheckNull(object value)
    {
        return value != null && !string.IsNullOrWhiteSpace(value.ToString()) ? value.ToString() : null;
    }

    public static string ValueOrEmpty(object value)
    {
        var result = value != null && !string.IsNullOrWhiteSpace(value?.ToString()) ? value.ToString() : default(string);
        return result ?? string.Empty;
    }
}
