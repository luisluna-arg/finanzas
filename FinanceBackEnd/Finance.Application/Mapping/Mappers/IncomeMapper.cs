using Finance.Application.Dtos.Incomes;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class IncomeMapper : BaseMapper<Income, IncomeDto>, IIncomeMapper
{
    public IncomeMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIncomeMapper : IMapper<Income, IncomeDto>
{
}
