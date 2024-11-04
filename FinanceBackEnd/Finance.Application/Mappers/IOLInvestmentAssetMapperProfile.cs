using Finance.Application.Dtos.IOLInvestmentAssets;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class IOLInvestmentAssetMapperProfile : BaseEntityMapperProfile<IOLInvestmentAsset, IOLInvestmentAssetDto>
{
    public IOLInvestmentAssetMapperProfile()
        : base()
    {
    }
}
