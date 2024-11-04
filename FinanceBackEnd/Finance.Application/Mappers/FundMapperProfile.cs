using Finance.Application.Dtos.Funds;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class FundMapperProfile : BaseEntityMapperProfile<Fund, FundDto>
{
    public FundMapperProfile()
        : base()
    {
    }
}
