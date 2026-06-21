using CQRSDispatch;
using Finance.Application.Auth;

namespace Finance.Application.Specifications.CreditCards;

public class CanSetSystemFlag(IsAdminUser isAdminUser)
{
    public async Task<CommandResult> IsSatisfiedAsync(CancellationToken ct = default)
    {
        var (isAdmin, _) = await isAdminUser.IsSatisfiedAsync(ct);
        return isAdmin
            ? CommandResult.Success()
            : CommandResult.Failure("Only admins can set the system flag on templates.");
    }
}
