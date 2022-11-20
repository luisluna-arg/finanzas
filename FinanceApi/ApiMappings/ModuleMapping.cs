namespace FinanceApi.ApiMappings;
internal static class ModuleMappingExtensions
{
    private static string route = "/module";
    internal static void ModuleMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDb db) => db.Module);
    }
}
