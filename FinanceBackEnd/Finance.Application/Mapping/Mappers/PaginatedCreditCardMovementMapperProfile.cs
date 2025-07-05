using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedCreditCardMovementMapper : PaginatedResultMapper<CreditCardMovement, CreditCardMovementDto>, IPaginatedCreditCardMovementMapper
{
    public PaginatedCreditCardMovementMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedCreditCardMovementMapper : IPaginatedResultMapper<CreditCardMovement, CreditCardMovementDto>
{
}
