using Finance.Application.Legacy.Dtos.IOLInvestmentAssetTypes;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class IOLInvestmentAssetTypeMapper : BaseMapper<IOLInvestmentAssetType, IOLInvestmentAssetTypeDto>, IIOLInvestmentAssetTypeMapper
{
    public IOLInvestmentAssetTypeMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIOLInvestmentAssetTypeMapper : IMapper<IOLInvestmentAssetType, IOLInvestmentAssetTypeDto>;
