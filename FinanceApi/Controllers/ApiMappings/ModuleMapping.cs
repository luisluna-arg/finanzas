using FinanceApi.Domain;

namespace FinanceApi.Controllers.ApiMappings;

internal static class ModuleMappingExtensions
{
    private static string route = "/module";
    internal static void ModuleMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDbContext db) => db.Module);
    }
}
