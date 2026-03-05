using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class CreditCardStatementTransactionMapper : BaseMapper<CreditCardStatementTransaction, CreditCardStatementTransactionDto>, ICreditCardStatementTransactionMapper
{
    public CreditCardStatementTransactionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardStatementTransactionMapper : IMapper<CreditCardStatementTransaction, CreditCardStatementTransactionDto>;
