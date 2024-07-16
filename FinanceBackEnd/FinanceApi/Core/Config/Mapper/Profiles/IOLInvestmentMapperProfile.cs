using FinanceApi.Application.Dtos.IOLInvestments;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class IOLInvestmentMapperProfile : BaseEntityMapperProfile<IOLInvestment, IOLInvestmentDto>
{
    public IOLInvestmentMapperProfile()
        : base()
    {
    }
}
