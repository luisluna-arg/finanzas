using Finance.Application.Dtos.IOLInvestmentAssets;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class IOLInvestmentAssetMapper : BaseMapper<IOLInvestmentAsset, IOLInvestmentAssetDto>, IIOLInvestmentAssetMapper
{
    public IOLInvestmentAssetMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIOLInvestmentAssetMapper : IMapper<IOLInvestmentAsset, IOLInvestmentAssetDto>
{
}
