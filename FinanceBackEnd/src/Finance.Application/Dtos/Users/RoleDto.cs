using Finance.Application.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Dtos.Users;

public record RoleDto : KeyValueEntityDto<RoleEnum>
{
    public RoleDto()
    {
    }
}
