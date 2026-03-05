using Finance.Application.Legacy.Dtos.Incomes;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Incomes;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class IncomeMapper : BaseMapper<Income, IncomeDto>, IIncomeMapper
{
    public IncomeMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIncomeMapper : IMapper<Income, IncomeDto>;
