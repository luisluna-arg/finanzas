using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class CreditCardMapperProfile : BaseEntityMapperProfile<CreditCard, CreditCardDto>
{
    public CreditCardMapperProfile()
        : base()
    {
        this.Map.ForMember(o => o.RecordCount, o => o.MapFrom(x => x.Movements.Count));
    }
}
