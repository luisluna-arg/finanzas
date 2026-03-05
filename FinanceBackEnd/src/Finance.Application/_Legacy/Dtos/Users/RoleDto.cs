using Finance.Application.Legacy.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Legacy.Dtos.Users;

public record RoleDto : KeyValueEntityDto<RoleEnum>
{
    public RoleDto()
    {
    }
}
