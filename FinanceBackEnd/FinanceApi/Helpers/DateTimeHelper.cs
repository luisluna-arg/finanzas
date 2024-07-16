using System.Globalization;

namespace FinanceApi.Helpers;

public static class DateTimeHelper
{
    public static int GetMonthNumber(string monthName, CultureInfo? culture = null)
    {
        return DateTime.ParseExact(monthName.Trim(), "MMMM", culture ?? CultureInfo.CurrentCulture).Month;
    }

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

    public static DateTime ParseDateTime(object? value, string format, IFormatProvider? formatProvider = null, DateTimeKind kind = DateTimeKind.Unspecified)
        => ParseNullDateTime(value, format, formatProvider, kind) ?? default;

    internal static DateTime FromTimeZoneToUTC(DateTime currentDate, short offset)
    {
        TimeSpan timeZoneOffset = TimeSpan.FromHours(offset);

        var timezoneStr = $"UTC{offset}";
        TimeZoneInfo timeZone = TimeZoneInfo.CreateCustomTimeZone(
            timezoneStr,
            timeZoneOffset,
            timezoneStr,
            timezoneStr);

        var currentDateWithNoKind = DateTime.SpecifyKind(currentDate, DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(currentDateWithNoKind, timeZone);
    }
}
