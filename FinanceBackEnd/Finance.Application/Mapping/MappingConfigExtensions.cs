using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Mapping;

public static class MappingConfigExtensions
{
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddScoped<IMappingService, MappingService>();
    }
}
