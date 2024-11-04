using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class DebitOriginMapperProfile : BaseEntityMapperProfile<DebitOrigin, DebitOriginDto>
{
    public DebitOriginMapperProfile()
        : base()
    {
        this.Map.ForMember(o => o.RecordCount, o => o.MapFrom(x => x.Debits.Count));
    }
}
