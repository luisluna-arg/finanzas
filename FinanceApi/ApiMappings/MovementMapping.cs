namespace FinanceApi.ApiMappings;
internal static class MovementMappingExtensions
{
    private static string route = "/movement";
    internal static void MovementMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDb db) => db.Movement);
    }
}
