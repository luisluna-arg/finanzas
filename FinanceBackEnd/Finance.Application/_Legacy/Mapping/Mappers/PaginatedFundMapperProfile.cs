using Finance.Application.Legacy.Dtos.Funds;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Funds;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class PaginatedFundMapper : PaginatedResultMapper<Fund, FundDto>, IPaginatedFundMapper
{
    public PaginatedFundMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedFundMapper : IPaginatedResultMapper<Fund, FundDto>;
