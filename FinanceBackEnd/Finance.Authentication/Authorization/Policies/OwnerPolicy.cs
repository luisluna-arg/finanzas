using Finance.Authentication.Authorization.Base;
using Finance.Domain.Enums;

namespace Finance.Authentication.Authorization.Policies;

/// <summary>
/// Authorization policy for Owner users.
/// </summary>
public class OwnerPolicy : RolePolicy
{
    public OwnerPolicy(IServiceProvider serviceProvider)
        : base("OwnerPolicy", serviceProvider, [RoleEnum.Owner])
    {
    }
}
