using Finance.Api.Core.Authorization.Base;
using Finance.Domain.Enums;

namespace Finance.Api.Core.Authorization.Policies;

/// <summary>
/// Authorization policy for Admin users.
/// </summary>
public class AdminPolicy : RolePolicy
{
    public AdminPolicy(Persistance.FinanceDbContext dbContext)
        : base("AdminPolicy", [RoleEnum.Admin], dbContext)
    {
    }
}
