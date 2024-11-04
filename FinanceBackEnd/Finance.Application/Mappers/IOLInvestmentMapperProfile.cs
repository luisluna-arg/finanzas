using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class IOLInvestmentMapperProfile : BaseEntityMapperProfile<IOLInvestment, IOLInvestmentDto>
{
    public IOLInvestmentMapperProfile()
        : base()
    {
    }
}
