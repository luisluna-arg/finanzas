using Finance.Application.Dtos.Funds;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Funds;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedFundMapper : PaginatedResultMapper<Fund, FundDto>, IPaginatedFundMapper
{
    public PaginatedFundMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedFundMapper : IPaginatedResultMapper<Fund, FundDto>;
