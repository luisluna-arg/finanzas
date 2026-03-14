using Finance.Application.Dtos.Identities;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Enums;

namespace Finance.Application.Commands.Users;

public class CreateUserSagaRequest : CreateUserCommand, ISagaRequest
{
    public IEnumerable<IdentityDto> Identities { get; set; }
    public CreateUserSagaRequest(string username, string firstName, string lastName, IEnumerable<RoleEnum> roles, IEnumerable<IdentityDto> identities)
        : base()
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Roles = roles;
        Identities = identities;
    }
}
