using Finance.Persistence.Services;

namespace Finance.Authentication.Services;

public class AuthDbContextService : BaseDbContextService
{
    public AuthDbContextService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
