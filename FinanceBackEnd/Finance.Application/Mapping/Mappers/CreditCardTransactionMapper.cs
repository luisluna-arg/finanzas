using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class CreditCardTransactionMapper : BaseMapper<CreditCardTransaction, CreditCardTransactionDto>, ICreditCardTransactionMapper
{
    public CreditCardTransactionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardTransactionMapper : IMapper<CreditCardTransaction, CreditCardTransactionDto>
{
}
