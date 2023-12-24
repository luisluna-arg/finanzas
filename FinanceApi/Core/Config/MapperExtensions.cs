using FinanceApi.Core.Config.Mapper.Profiles;

namespace FinanceApi.Core.Config;

public static class MapperExtensions
{
    public static void AddAutoMapping(this IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(AppModuleMapperProfile))
            .AddAutoMapper(typeof(AppModuleBasicMapperProfile))
            .AddAutoMapper(typeof(AppModuleTypeMapperProfile))
            .AddAutoMapper(typeof(BankMapperProfile))
            .AddAutoMapper(typeof(CreditCardMapperProfile))
            .AddAutoMapper(typeof(CreditCardMovementMapperProfile))
            .AddAutoMapper(typeof(CurrencyConversionMapperProfile))
            .AddAutoMapper(typeof(CurrencyMapperProfile))
            .AddAutoMapper(typeof(DebitMapperProfile))
            .AddAutoMapper(typeof(DebitOriginMapperProfile))
            .AddAutoMapper(typeof(IOLInvestmentAssetMapperProfile))
            .AddAutoMapper(typeof(IOLInvestmentAssetTypeMapperProfile))
            .AddAutoMapper(typeof(IOLInvestmentMapperProfile))
            .AddAutoMapper(typeof(MovementMapperProfile))
            .AddAutoMapper(typeof(PaginatedDebitMapperProfile))
            .AddAutoMapper(typeof(PaginatedMovementMapperProfile))
            ;
    }
}
