using FinanceApi.Core.Config.Mapper.Profiles;

namespace FinanceApi.Core.Config;

public static class MapperExtensions
{
    public static void AddAutoMapping(this IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(AppModuleMapperProfile))
            .AddAutoMapper(typeof(BankMapperProfile))
            .AddAutoMapper(typeof(CurrencyConversionMapperProfile))
            .AddAutoMapper(typeof(CurrencyMapperProfile))
            .AddAutoMapper(typeof(InvestmentAssetIOLMapperProfile))
            .AddAutoMapper(typeof(InvestmentAssetIOLTypeTypeMapperProfile))
            .AddAutoMapper(typeof(MovementMapperProfile));
    }
}
