namespace FinanceApi.Controllers.ApiMappings;
internal static class BankMappingExtensions
{
    private static string route = "/bank";
    internal static void BankMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDbContext db) => db.Bank);
    }
}
