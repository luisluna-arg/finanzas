using Finance.Application.Legacy.Dtos.Users;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Auth;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class RoleMapper : BaseMapper<Role, RoleDto>, IRoleMapper
{
    public RoleMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IRoleMapper : IMapper<Role, RoleDto>;
