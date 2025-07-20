using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class CreditCardMovementMapper : BaseMapper<CreditCardMovement, CreditCardMovementDto>, ICreditCardMovementMapper
{
    public CreditCardMovementMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardMovementMapper : IMapper<CreditCardMovement, CreditCardMovementDto>
{
}
