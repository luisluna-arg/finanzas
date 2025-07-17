using Finance.Domain.Enums;
using Finance.Domain.Models;

namespace Finance.Api.Controllers.Requests;

public abstract class BaseUserRequest
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IEnumerable<RoleEnum> Roles { get; set; }
    public IEnumerable<Identity> Identities { get; set; } = [];
}