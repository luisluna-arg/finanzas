using System.Globalization;

namespace FinanceApi.Helpers;

public static class ParsingHelper
{
    public static int? ParseNullInteger(object? value)
    {
        if (value != null)
        {
            var valueStr = value.ToString();
            if (!string.IsNullOrWhiteSpace(valueStr) && int.TryParse(valueStr, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
        }

        return default;
    }

    public static uint? ParseNullUInteger(object? value)
    {
        if (value != null)
        {
            var valueStr = value.ToString();
            if (!string.IsNullOrWhiteSpace(valueStr) && uint.TryParse(valueStr, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
        }

        return default;
    }

    public static decimal? ParseNullDecimal(object? value)
    {
        if (value != null)
        {
            var valueStr = value.ToString();
            if (!string.IsNullOrWhiteSpace(valueStr) && decimal.TryParse(valueStr, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
        }

        return default;
    }

    public static uint ParseUInteger(object? value) => ParseNullUInteger(value) ?? default;

    public static int ParseInteger(object? value) => ParseNullInteger(value) ?? default;

    public static decimal ParseDecimal(object? value) => ParseNullDecimal(value) ?? default;
}
