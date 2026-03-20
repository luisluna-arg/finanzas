using Finance.Helpers;

namespace Finance.Application.Tests.Helpers;

public class DateTimeHelperTests
{
    [Fact]
    public void ParseDateTime_ValidString_ReturnsParsedDate()
    {
        var result = DateTimeHelper.ParseDateTime("1/6/2025 12:30:00", "d/M/yyyy HH:mm:ss");

        Assert.Equal(new DateTime(2025, 6, 1, 12, 30, 0), result);
    }

    [Fact]
    public void ParseDateTime_InvalidString_ReturnsDefault()
    {
        var result = DateTimeHelper.ParseDateTime("not-a-date", "d/M/yyyy HH:mm:ss");

        Assert.Equal(default, result);
    }

    [Fact]
    public void ParseDateTime_NullValue_ReturnsDefault()
    {
        var result = DateTimeHelper.ParseDateTime(null, "d/M/yyyy HH:mm:ss");

        Assert.Equal(default, result);
    }

    [Fact]
    public void FromTimeZoneToUTC_NegativeOffset_AddsOffsetToConvertToUTC()
    {
        var localTime = new DateTime(2025, 6, 1, 12, 0, 0);

        var result = DateTimeHelper.FromTimeZoneToUTC(localTime, -3);

        Assert.Equal(new DateTime(2025, 6, 1, 15, 0, 0, DateTimeKind.Utc), result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void FromTimeZoneToUTC_PositiveOffset_SubtractsOffsetToConvertToUTC()
    {
        var localTime = new DateTime(2025, 6, 1, 12, 0, 0);

        var result = DateTimeHelper.FromTimeZoneToUTC(localTime, 2);

        Assert.Equal(new DateTime(2025, 6, 1, 10, 0, 0, DateTimeKind.Utc), result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void FromTimeZoneToUTC_IgnoresExistingKind_TreatsAsUnspecified()
    {
        var utcLabelled = new DateTime(2025, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        var result = DateTimeHelper.FromTimeZoneToUTC(utcLabelled, -3);

        Assert.Equal(new DateTime(2025, 6, 1, 15, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void FromTimeZoneToUTC_ZeroOffset_ReturnsSameTimeAsUTC()
    {
        var localTime = new DateTime(2025, 6, 1, 12, 0, 0);

        var result = DateTimeHelper.FromTimeZoneToUTC(localTime, 0);

        Assert.Equal(new DateTime(2025, 6, 1, 12, 0, 0, DateTimeKind.Utc), result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }
}
