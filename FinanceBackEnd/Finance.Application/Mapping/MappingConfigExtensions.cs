using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Mapping;

public static class MappingConfigExtensions
{
    public static void AddDtoMappers(this IServiceCollection services)
    {
        services.AddScoped<IMappingService, MappingService>();
    }
}
