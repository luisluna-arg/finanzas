namespace Finance.Application.Extensions;

public static class DateExtensions
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

    public static DateTime CurrentMonth(this DateTime referenceDate)
    {
        return new DateTime(
                        referenceDate.Year,
                        referenceDate.Month,
                        1,
                        0,
                        0,
                        0,
                        referenceDate.Kind);
    }
}
