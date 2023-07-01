using FinanceApi.Domain;

namespace FinanceApi.Controllers.ApiMappings;

internal static class AppModuleMappingExtensions
{
    private static string route = "/app-module";
    internal static void AppModuleMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDbContext db) => db.AppModule);
    }
}
