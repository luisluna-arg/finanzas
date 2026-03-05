using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Legacy.Mapping;

public static class MappingConfigExtensions
{
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<IMappingService, MappingService>();
    }
}
