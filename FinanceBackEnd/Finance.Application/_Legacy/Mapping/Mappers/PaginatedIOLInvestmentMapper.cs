using Finance.Application.Legacy.Dtos.IOLInvestments;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class PaginatedIOLInvestmentMapper : PaginatedResultMapper<IOLInvestment, IOLInvestmentDto>, IPaginatedIOLInvestmentMapper
{
    public PaginatedIOLInvestmentMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedIOLInvestmentMapper : IPaginatedResultMapper<IOLInvestment, IOLInvestmentDto>;
