using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class CreditCardMapperProfile : BaseMapperProfile<CreditCard, CreditCardDto>
{
    public CreditCardMapperProfile()
        : base()
    {
        this.Map.ForMember(o => o.RecordCount, o => o.MapFrom(x => x.Movements.Count));
    }
}
