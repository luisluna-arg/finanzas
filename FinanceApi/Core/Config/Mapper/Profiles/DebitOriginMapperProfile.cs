using FinanceApi.Application.Dtos.DebitOrigins;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class DebitOriginMapperProfile : BaseEntityMapperProfile<DebitOrigin, DebitOriginDto>
{
    public DebitOriginMapperProfile()
        : base()
    {
        this.Map.ForMember(o => o.RecordCount, o => o.MapFrom(x => x.Debits.Count));
    }
}
