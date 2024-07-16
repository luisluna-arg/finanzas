using FinanceApi.Application.Dtos.IOLInvestmentAssets;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class IOLInvestmentAssetMapperProfile : BaseEntityMapperProfile<IOLInvestmentAsset, IOLInvestmentAssetDto>
{
    public IOLInvestmentAssetMapperProfile()
        : base()
    {
    }
}
