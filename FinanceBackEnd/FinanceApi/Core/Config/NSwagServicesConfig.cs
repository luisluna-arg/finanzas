namespace FinanceApi.Core.Config;

public static class NSwagServicesConfig
{
    public static void ConfigureNSwag(this IServiceCollection services)
    {
        services
            .AddOpenApiDocument(cfg =>
            {
                cfg.DocumentName = "v1";
                cfg.Title = "Finances API";
                cfg.Version = "v1";
            });
    }
}
