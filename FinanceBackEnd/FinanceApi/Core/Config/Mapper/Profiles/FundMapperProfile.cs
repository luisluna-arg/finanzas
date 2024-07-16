using FinanceApi.Application.Dtos.Funds;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class FundMapperProfile : BaseEntityMapperProfile<Fund, FundDto>
{
    public FundMapperProfile()
        : base()
    {
    }
}
