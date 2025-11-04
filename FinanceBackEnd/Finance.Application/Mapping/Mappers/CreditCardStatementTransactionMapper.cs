using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class CreditCardStatementTransactionMapper : BaseMapper<CreditCardStatementTransaction, CreditCardStatementTransactionDto>, ICreditCardStatementTransactionMapper
{
    public CreditCardStatementTransactionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardStatementTransactionMapper : IMapper<CreditCardStatementTransaction, CreditCardStatementTransactionDto>
{
}
