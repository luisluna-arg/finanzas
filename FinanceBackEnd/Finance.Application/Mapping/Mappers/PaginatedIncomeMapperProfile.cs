using Finance.Application.Dtos.Incomes;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Incomes;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedIncomeMapper : PaginatedResultMapper<Income, IncomeDto>, IPaginatedIncomeMapper
{
    public PaginatedIncomeMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedIncomeMapper : IPaginatedResultMapper<Income, IncomeDto>;
