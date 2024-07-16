using FinanceApi.Application.Dtos.Incomes;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class IncomeMapperProfile : BaseEntityMapperProfile<Income, IncomeDto>
{
    public IncomeMapperProfile()
        : base()
    {
    }
}
