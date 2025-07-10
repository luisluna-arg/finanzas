using Finance.Api.Core.Authorization.Base;
using Finance.Domain.Enums;

namespace Finance.Api.Core.Authorization.Policies;

/// <summary>
/// Authorization policy for users who are either Admins or Owners.
/// </summary>
public class AdminOrOwnerPolicy : RolePolicy
{
    public AdminOrOwnerPolicy(Persistance.FinanceDbContext dbContext)
        : base("AdminOrOwnerPolicy", [RoleEnum.Owner, RoleEnum.Admin], dbContext)
    {
    }
}
