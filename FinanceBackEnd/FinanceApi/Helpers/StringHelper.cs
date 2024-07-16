namespace FinanceApi.Helpers;

public static class StringHelper
{
    public static string? CheckNull(object value)
        => value != null && !string.IsNullOrWhiteSpace(value.ToString()) ? value.ToString() : null;

    public static string ValueOrEmpty(object value)
    {
        var result = value != null && !string.IsNullOrWhiteSpace(value?.ToString()) ? value.ToString() : default;
        return result ?? string.Empty;
    }
}
