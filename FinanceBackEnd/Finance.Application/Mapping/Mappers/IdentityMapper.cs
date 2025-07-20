using Finance.Application.Dtos.Identities;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class IdentityMapper : BaseMapper<Identity, IdentityDto>, IIdentityMapper
{
    public IdentityMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IIdentityMapper : IMapper<Identity, IdentityDto>
{
}
