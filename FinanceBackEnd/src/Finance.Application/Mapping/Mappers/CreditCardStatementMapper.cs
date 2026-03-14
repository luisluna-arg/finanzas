using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Mapping.Mappers;

public class CreditCardStatementMapper : BaseMapper<CreditCardStatement, CreditCardStatementDto>, ICreditCardStatementMapper
{
    public CreditCardStatementMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardStatementMapper : IMapper<CreditCardStatement, CreditCardStatementDto>;
