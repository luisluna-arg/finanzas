using FinanceApi.Core.Config.Mapper.Profiles;

namespace FinanceApi.Core.Config;

public static class MapperExtensions
{
    public static void AddAutoMapping(this IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(CurrencyMapperProfile))
            .AddAutoMapper(typeof(AppModuleMapperProfile));
    }
}
