using Finance.Application.Legacy.Dtos.Funds;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Funds;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class FundMapper : BaseMapper<Fund, FundDto>, IFundMapper
{
    public FundMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IFundMapper : IMapper<Fund, FundDto>;
