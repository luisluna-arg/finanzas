using Finance.Application.Services.Interfaces;
using Finance.Domain.Enums;
using Finance.Domain.Models;

namespace Finance.Application.Commands;

public class CreateUserSagaRequest : CreateUserCommand, ISagaRequest
{
    public IEnumerable<Identity> Identities { get; set; }

    public CreateUserSagaRequest(string username, string firstName, string lastName, IEnumerable<RoleEnum> roles, IEnumerable<Identity> identities)
        : base()
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Roles = roles;
        Identities = identities;
    }
}
