using Finance.Application.Legacy.Dtos.IOLInvestments;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class IOLInvestmentMapper : BaseMapper<IOLInvestment, IOLInvestmentDto>, IIOLInvestmentMapper
{
    public IOLInvestmentMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIOLInvestmentMapper : IMapper<IOLInvestment, IOLInvestmentDto>;
