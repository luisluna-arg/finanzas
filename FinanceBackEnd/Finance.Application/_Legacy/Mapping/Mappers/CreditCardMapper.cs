using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class CreditCardMapper : BaseMapper<CreditCard, CreditCardDto>, ICreditCardMapper
{
    public CreditCardMapper(IMappingService mappingService) : base(mappingService)
    {
        // TODO Fix this mapping
        // this.Map.ForMember(o => o.RecordCount, o => o.MapFrom(x => x.Movements.Count));
    }
}

public interface ICreditCardMapper : IMapper<CreditCard, CreditCardDto>;
