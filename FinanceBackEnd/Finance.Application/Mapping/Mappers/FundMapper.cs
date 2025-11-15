using Finance.Application.Dtos.Funds;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Funds;

namespace Finance.Application.Mapping.Mappers;

public class FundMapper : BaseMapper<Fund, FundDto>, IFundMapper
{
    public FundMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IFundMapper : IMapper<Fund, FundDto>;
