using Finance.Application.Legacy.Dtos.IOLInvestmentAssets;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class IOLInvestmentAssetMapper : BaseMapper<IOLInvestmentAsset, IOLInvestmentAssetDto>, IIOLInvestmentAssetMapper
{
    public IOLInvestmentAssetMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIOLInvestmentAssetMapper : IMapper<IOLInvestmentAsset, IOLInvestmentAssetDto>;
