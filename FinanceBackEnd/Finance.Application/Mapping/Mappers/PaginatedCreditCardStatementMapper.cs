using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedCreditCardStatementMapper : PaginatedResultMapper<CreditCardStatement, CreditCardStatementDto>, IPaginatedCreditCardStatementMapper
{
    public PaginatedCreditCardStatementMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedCreditCardStatementMapper : IPaginatedResultMapper<CreditCardStatement, CreditCardStatementDto>;
