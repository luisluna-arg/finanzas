using Finance.Authentication.Authorization.Base;
using Finance.Domain.Enums;

namespace Finance.Authentication.Authorization.Policies;

/// <summary>
/// Authorization policy for Owner users.
/// </summary>
public class OwnerPolicy : RolePolicy
{
    public OwnerPolicy(Finance.Persistance.FinanceDbContext dbContext)
        : base("OwnerPolicy", [RoleEnum.Owner], dbContext)
    {
    }
}
