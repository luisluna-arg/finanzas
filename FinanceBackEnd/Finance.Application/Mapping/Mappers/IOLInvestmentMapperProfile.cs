using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Mapping.Mappers;

public class IOLInvestmentMapper : BaseMapper<IOLInvestment, IOLInvestmentDto>, IIOLInvestmentMapper
{
    public IOLInvestmentMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIOLInvestmentMapper : IMapper<IOLInvestment, IOLInvestmentDto>;
