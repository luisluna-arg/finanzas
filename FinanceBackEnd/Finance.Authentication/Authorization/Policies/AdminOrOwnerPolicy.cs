using Finance.Authentication.Authorization.Base;
using Finance.Domain.Enums;

namespace Finance.Authentication.Authorization.Policies;

/// <summary>
/// Authorization policy for users with either Admin or Owner role.
/// </summary>
public class AdminOrOwnerPolicy : RolePolicy
{
    public AdminOrOwnerPolicy(Finance.Persistance.FinanceDbContext dbContext)
        : base("AdminOrOwnerPolicy", [RoleEnum.Admin, RoleEnum.Owner], dbContext)
    {
    }
}
