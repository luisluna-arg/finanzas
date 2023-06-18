using FinanceApi.Core.Config.Mapper;

namespace FinanceApi.Core.Config;

public static class MapperExtensions
{
    public static void AddAutoMapping(this IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(CurrencyMapperProfile))
            .AddAutoMapper(typeof(ModuleMapperProfile));
    }
}
