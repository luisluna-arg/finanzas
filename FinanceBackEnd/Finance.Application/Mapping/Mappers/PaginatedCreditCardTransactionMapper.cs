using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedCreditCardTransactionMapper : PaginatedResultMapper<CreditCardTransaction, CreditCardTransactionDto>, IPaginatedCreditCardTransactionMapper
{
    public PaginatedCreditCardTransactionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedCreditCardTransactionMapper : IPaginatedResultMapper<CreditCardTransaction, CreditCardTransactionDto>;
