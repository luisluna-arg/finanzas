using Finance.Domain.Models;
using DispatchContext = CQRSDispatch.DispatchContext;

namespace Finance.Application.Auth;

public class FinanceDispatchContext : DispatchContext
{
    public FinanceDispatchContext()
    {
        UserInfo = new()
        {
            Id = Guid.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            Username = string.Empty,
            Roles = new List<Role>(),
            CreatedAt = DateTime.MinValue,
            UpdatedAt = DateTime.MinValue
        };
    }

    public User UserInfo { get; set; }
}
