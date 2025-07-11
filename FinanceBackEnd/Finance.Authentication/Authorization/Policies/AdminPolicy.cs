using Finance.Authentication.Authorization.Base;
using Finance.Domain.Enums;

namespace Finance.Authentication.Authorization.Policies;

/// <summary>
/// Authorization policy for Admin users.
/// </summary>
public class AdminPolicy : RolePolicy
{
    public AdminPolicy(Finance.Persistance.FinanceDbContext dbContext)
        : base("AdminPolicy", [RoleEnum.Admin], dbContext)
    {
    }
}
