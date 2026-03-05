using Finance.Authentication.Authorization.Base;

namespace Finance.Authentication.Authorization.Policies;

public class AuthenticatedPolicy : BasicPolicy
{
    public AuthenticatedPolicy()
        : base("AuthenticatedPolicy")
    {
    }
}
