using System.Globalization;

namespace FinanceApi.Helpers;

public static class ParsingHelper
{
    public static DateTime? ParseNullDateTime(object value, string format, IFormatProvider? formatProvider = null, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        if (value == null) return default(DateTime?);

        var valueStr = value.ToString();
        if (string.IsNullOrWhiteSpace(valueStr)) return default(DateTime?);

        DateTime result;
        if (DateTime.TryParseExact(valueStr, format, formatProvider, DateTimeStyles.None, out result))
        {
            return DateTime.SpecifyKind(result, dateTimeKind);
        }
        else
        {
            return default(DateTime?);
        }
    }

    public static DateTime ParseDateTime(object value, string format, IFormatProvider? formatProvider = null, DateTimeKind kind = DateTimeKind.Unspecified)
    {
        var result = ParseNullDateTime(value, format, formatProvider, kind);
        return result.HasValue ? result.Value : default(DateTime);
    }

    public static decimal? ParseNullDecimal(object value)
    {
        if (value == null) return default(decimal);

        var valueStr = value.ToString();
        if (string.IsNullOrWhiteSpace(valueStr)) return default(decimal);

        decimal result;
        return decimal.TryParse(valueStr, out result) ? result : default(decimal);
    }

    public static decimal ParseDecimal(object value)
    {
        var result = ParseNullDecimal(value);
        return result.HasValue ? result.Value : default(decimal);
    }
}
