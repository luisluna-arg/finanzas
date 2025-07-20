using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class DebitOriginMapper : BaseMapper<DebitOrigin, DebitOriginDto>, IDebitOriginMapper
{
    public DebitOriginMapper(IMappingService mappingService) : base(mappingService)
    {
        // this.Map.ForMember(o => o.RecordCount, o => o.MapFrom(x => x.Debits.Count));
    }
}

public interface IDebitOriginMapper : IMapper<DebitOrigin, DebitOriginDto>
{
}
