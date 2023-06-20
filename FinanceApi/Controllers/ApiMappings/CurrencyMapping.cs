using FinanceApi.Domain;

namespace FinanceApi.Controllers.ApiMappings;

internal static class CurrencyMappingExtensions
{
    private static string route = "/currency";
    internal static void CurrencyMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDbContext db) => db.Currency);
    }
}
