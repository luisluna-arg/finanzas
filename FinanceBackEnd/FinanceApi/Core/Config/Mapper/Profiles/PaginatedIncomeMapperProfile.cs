using FinanceApi.Application.Dtos.Incomes;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class PaginatedIncomeMapperProfile : PaginatedResultMapperProfile<Income, IncomeDto>
{
    public PaginatedIncomeMapperProfile()
        : base()
    {
    }
}