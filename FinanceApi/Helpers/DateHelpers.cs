using System.Globalization;

namespace FinanceApi.Extensions;

internal static class DateHelpers
{
    public static int GetMonthNumber(string monthName, CultureInfo? culture = null)
    {
        return DateTime.ParseExact(monthName.Trim(), "MMMM", culture ?? CultureInfo.CurrentCulture).Month;
    }
}
