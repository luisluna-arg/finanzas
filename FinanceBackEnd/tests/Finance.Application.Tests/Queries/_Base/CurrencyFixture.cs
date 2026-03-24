using Finance.Domain.Models.Currencies;

namespace Finance.Application.Tests.Queries.Base;

/// <summary>
/// Builds Currency instances with short names that never clash with pre-seeded currencies (ARS, USD).
/// </summary>
public static class CurrencyFixture
{
    private static readonly string[] _shortNames =
    [
        "EUR", "GBP", "JPY", "CHF", "CAD", "BRL", "MXN", "CNY", "KRW", "INR",
        "RUB", "TRY", "ZAR", "SEK", "NOK", "DKK", "PLN", "CZK", "HUF", "RON"
    ];

    private static int _index;

    private static string NextShortName()
    {
        var name = _shortNames[_index % _shortNames.Length];
        _index++;
        return name;
    }

    public static Currency Build(
        string? shortName = null,
        string? name = null,
        bool deactivated = false,
        ICollection<CurrencySymbol>? symbols = null)
    {
        var sn = shortName ?? NextShortName();
        return new Currency
        {
            Id = Guid.NewGuid(),
            ShortName = sn,
            Name = name ?? sn,
            Deactivated = deactivated,
            Symbols = symbols ?? [],
        };
    }

    public static Currency[] BuildMany(int count, bool deactivated = false) =>
        Enumerable.Range(0, count).Select(_ => Build(deactivated: deactivated)).ToArray();
}
