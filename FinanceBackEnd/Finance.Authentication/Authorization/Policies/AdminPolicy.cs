using Finance.Authentication.Authorization.Base;
using Finance.Domain.Enums;
using Finance.Persistance;

namespace Finance.Authentication.Authorization.Policies;

/// <summary>
/// Authorization policy for Admin users.
/// </summary>
public class AdminPolicy : RolePolicy
{
    public AdminPolicy(FinanceDbContext dbContext)
        : base("AdminPolicy", [RoleEnum.Admin], dbContext)
    {
    }
}
