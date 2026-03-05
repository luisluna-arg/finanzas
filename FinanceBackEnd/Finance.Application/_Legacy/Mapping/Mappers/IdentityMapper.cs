using Finance.Application.Legacy.Dtos.Identities;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Identities;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class IdentityMapper : BaseMapper<Identity, IdentityDto>, IIdentityMapper
{
    public IdentityMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIdentityMapper : IMapper<Identity, IdentityDto>;
