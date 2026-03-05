using Finance.Application.Legacy.Dtos.Identities;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Domain.Enums;

namespace Finance.Application.Legacy.Commands.Users;

public class UpdateUserSagaRequest : UpdateUserCommand, ISagaRequest
{
    public IEnumerable<IdentityDto> Identities { get; set; }

    public UpdateUserSagaRequest(Guid userId, string username, string firstName, string lastName, IEnumerable<RoleEnum> roles, IEnumerable<IdentityDto> identities)
    {
        UserId = userId;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Roles = roles;
        Identities = identities;
    }
}
