namespace FinanceApi.ApiMappings;
internal static class FundMappingExtensions
{
    private static string route = "/fund";
    internal static void FundMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDb db) => db.Fund);
    }
}
