using Finance.Application.Dtos.IOLInvestmentAssetTypes;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class IOLInvestmentAssetTypeMapper : BaseMapper<IOLInvestmentAssetType, IOLInvestmentAssetTypeDto>, IIOLInvestmentAssetTypeMapper
{
    public IOLInvestmentAssetTypeMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIOLInvestmentAssetTypeMapper : IMapper<IOLInvestmentAssetType, IOLInvestmentAssetTypeDto>
{
}
