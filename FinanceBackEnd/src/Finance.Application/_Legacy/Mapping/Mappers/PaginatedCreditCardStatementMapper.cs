using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class PaginatedCreditCardStatementMapper : PaginatedResultMapper<CreditCardStatement, CreditCardStatementDto>, IPaginatedCreditCardStatementMapper
{
    public PaginatedCreditCardStatementMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedCreditCardStatementMapper : IPaginatedResultMapper<CreditCardStatement, CreditCardStatementDto>;
