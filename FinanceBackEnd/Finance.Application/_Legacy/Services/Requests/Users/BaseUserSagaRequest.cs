using Finance.Application.Legacy.Dtos.Identities;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Domain.Enums;

namespace Finance.Application.Legacy.Commands.Users;

public abstract class BaseUserSagaRequest : CreateUserCommand, ISagaRequest
{
    public IEnumerable<IdentityDto> Identities { get; set; }

    public BaseUserSagaRequest(string username, string firstName, string lastName, IEnumerable<RoleEnum> roles, IEnumerable<IdentityDto> identities)
        : base()
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Roles = roles;
        Identities = identities;
    }
}
