using Finance.Application.Services.Interfaces;
using Finance.Domain.Enums;
using Finance.Domain.Models;

namespace Finance.Application.Commands;

public class UpdateUserSagaRequest : UpdateUserCommand, ISagaRequest
{
    public IEnumerable<Identity> Identities { get; set; }

    public UpdateUserSagaRequest(Guid userId, string username, string firstName, string lastName, IEnumerable<RoleEnum> roles, IEnumerable<Identity> identities)
    {
        UserId = userId;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Roles = roles;
        Identities = identities;
    }
}
