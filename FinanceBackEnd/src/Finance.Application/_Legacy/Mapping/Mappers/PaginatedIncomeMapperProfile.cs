using Finance.Application.Legacy.Dtos.Incomes;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Incomes;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class PaginatedIncomeMapper : PaginatedResultMapper<Income, IncomeDto>, IPaginatedIncomeMapper
{
    public PaginatedIncomeMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedIncomeMapper : IPaginatedResultMapper<Income, IncomeDto>;
