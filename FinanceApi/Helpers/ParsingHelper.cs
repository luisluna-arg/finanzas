using System.Globalization;

namespace FinanceApi.Helpers;

public static class ParsingHelper
{
    public static DateTime? ParseNullDateTime(object? value, string format, IFormatProvider? formatProvider = null, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        if (value != null)
        {
            var valueStr = value.ToString();
            if (!string.IsNullOrWhiteSpace(valueStr) &&
                DateTime.TryParseExact(valueStr, format, formatProvider, DateTimeStyles.None, out var result))
            {
                return DateTime.SpecifyKind(result, dateTimeKind);
            }
        }

        return default;
    }

    public static int? ParseNullInteger(object? value)
    {
        if (value != null)
        {
            var valueStr = value.ToString();
            if (!string.IsNullOrWhiteSpace(valueStr) && int.TryParse(valueStr, out var result))
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
            if (!string.IsNullOrWhiteSpace(valueStr) && uint.TryParse(valueStr, out var result))
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
            if (!string.IsNullOrWhiteSpace(valueStr) && decimal.TryParse(value.ToString(), out var result))
            {
                return result;
            }
        }

        return default;
    }

    public static DateTime ParseDateTime(object? value, string format, IFormatProvider? formatProvider = null, DateTimeKind kind = DateTimeKind.Unspecified)
        => ParseNullDateTime(value, format, formatProvider, kind) ?? default;

    public static uint ParseUInteger(object? value) => ParseNullUInteger(value) ?? default;

    public static int ParseInteger(object? value) => ParseNullInteger(value) ?? default;

    public static decimal ParseDecimal(object? value) => ParseNullDecimal(value) ?? default;
}
