using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class CreditCardMapper : BaseMapper<CreditCard, CreditCardDto>, ICreditCardMapper
{
    public CreditCardMapper(IMappingService mappingService) : base(mappingService)
    {
        // this.Map.ForMember(o => o.RecordCount, o => o.MapFrom(x => x.Movements.Count));
    }
}

public interface ICreditCardMapper : IMapper<CreditCard, CreditCardDto>
{
}
