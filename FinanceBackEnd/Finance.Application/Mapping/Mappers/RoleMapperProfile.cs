using Finance.Application.Dtos.Users;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mapping.Mappers;

public class RoleMapper : BaseMapper<Role, RoleDto>, IRoleMapper
{
    public RoleMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IRoleMapper : IMapper<Role, RoleDto>
{
}
