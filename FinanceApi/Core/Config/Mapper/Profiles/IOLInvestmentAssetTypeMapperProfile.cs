using FinanceApi.Application.Dtos.IOLInvestmentAssetTypes;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class IOLInvestmentAssetTypeMapperProfile : BaseMapperProfile<IOLInvestmentAssetType, IOLInvestmentAssetTypeDto>
{
    public IOLInvestmentAssetTypeMapperProfile()
        : base()
    {
    }
}
