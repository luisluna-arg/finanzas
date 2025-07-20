using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedIOLInvestmentMapper : PaginatedResultMapper<IOLInvestment, IOLInvestmentDto>, IPaginatedIOLInvestmentMapper
{
    public PaginatedIOLInvestmentMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedIOLInvestmentMapper : IPaginatedResultMapper<IOLInvestment, IOLInvestmentDto>
{
}
