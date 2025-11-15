using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Mapping.Mappers;

public class DebitOriginMapper : BaseMapper<DebitOrigin, DebitOriginDto>, IDebitOriginMapper
{
    public DebitOriginMapper(IMappingService mappingService) : base(mappingService)
    {
        // TODO Fix this mapping
        // this.Map.ForMember(o => o.RecordCount, o => o.MapFrom(x => x.Debits.Count));
    }
}

public interface IDebitOriginMapper : IMapper<DebitOrigin, DebitOriginDto>;
