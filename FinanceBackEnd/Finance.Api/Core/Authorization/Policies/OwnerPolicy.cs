using Finance.Api.Core.Authorization.Base;
using Finance.Domain.Enums;

namespace Finance.Api.Core.Authorization.Policies;

/// <summary>
/// Authorization policy for Owner users.
/// </summary>
public class OwnerPolicy : RolePolicy
{
    public OwnerPolicy(Persistance.FinanceDbContext dbContext)
        : base("OwnerPolicy", [RoleEnum.Owner], dbContext)
    {
    }
}
