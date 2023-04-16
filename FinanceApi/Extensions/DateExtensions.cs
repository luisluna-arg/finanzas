namespace FinanceApi.Extensions;

internal static class DateExtensions
{
    public static DateTime Duplicate(this DateTime referenceDate)
    {
        return new DateTime(
                        referenceDate.Year,
                        referenceDate.Month,
                        referenceDate.Day,
                        referenceDate.Hour,
                        referenceDate.Minute,
                        referenceDate.Second,
                        referenceDate.Kind);
    }
}
