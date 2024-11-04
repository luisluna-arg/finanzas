using Finance.Application.Dtos.Incomes;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class IncomeMapperProfile : BaseEntityMapperProfile<Income, IncomeDto>
{
    public IncomeMapperProfile()
        : base()
    {
    }
}
