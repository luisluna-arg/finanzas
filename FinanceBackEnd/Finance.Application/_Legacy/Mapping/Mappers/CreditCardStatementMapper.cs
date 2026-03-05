using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class CreditCardStatementMapper : BaseMapper<CreditCardStatement, CreditCardStatementDto>, ICreditCardStatementMapper
{
    public CreditCardStatementMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardStatementMapper : IMapper<CreditCardStatement, CreditCardStatementDto>;
