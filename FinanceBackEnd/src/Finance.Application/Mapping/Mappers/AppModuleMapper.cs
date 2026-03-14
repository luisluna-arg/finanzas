using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.AppModules;

namespace Finance.Application.Mapping.Mappers;

public class AppModuleMapper : BaseMapper<AppModule, AppModuleDto>, IAppModuleMapper
{
    public AppModuleMapper(IMappingService mappingService) : base(mappingService)
    {
    }

    public override AppModuleDto Map(AppModule source)
    {
        var target = base.Map(source);

        if (source.Currency != null)
        {
            target.CurrencyId = source.Currency.Id;
            // The Currency and Type properties will be mapped by DtoMappingService if it's configured
        }

        return target;
    }
}

public interface IAppModuleMapper : IMapper<AppModule, AppModuleDto>;
