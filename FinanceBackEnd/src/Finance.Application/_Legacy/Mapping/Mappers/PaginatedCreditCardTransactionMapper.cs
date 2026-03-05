using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class PaginatedCreditCardTransactionMapper : PaginatedResultMapper<CreditCardTransaction, CreditCardTransactionDto>, IPaginatedCreditCardTransactionMapper
{
    public PaginatedCreditCardTransactionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedCreditCardTransactionMapper : IPaginatedResultMapper<CreditCardTransaction, CreditCardTransactionDto>;
