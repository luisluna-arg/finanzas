using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class CreditCardMovementMapperProfile : BaseEntityMapperProfile<CreditCardMovement, CreditCardMovementDto>
{
    public CreditCardMovementMapperProfile()
        : base()
    {
    }
}
