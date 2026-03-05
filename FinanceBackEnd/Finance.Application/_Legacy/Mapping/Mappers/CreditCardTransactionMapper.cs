using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class CreditCardTransactionMapper : BaseMapper<CreditCardTransaction, CreditCardTransactionDto>, ICreditCardTransactionMapper
{
    public CreditCardTransactionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardTransactionMapper : IMapper<CreditCardTransaction, CreditCardTransactionDto>;
