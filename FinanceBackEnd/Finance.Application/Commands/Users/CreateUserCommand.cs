using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Commands.Users;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Commands;

public class CreateUserCommand : BaseUserCommand;

public class CreateUserCommandHandler : BaseCommandHandler<CreateUserCommand, User>
{
    public CreateUserCommandHandler(FinanceDbContext dbContext) : base(dbContext) { }

    public async override Task<DataResult<User>> ExecuteAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var roles = DbContext.Role
            .Where(r => command.Roles.Contains(r.Id))
            .ToList();

        var user = new User
        {
            Username = command.Username,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Roles = roles
        };
        DbContext.User.Add(user);

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<User>.Success(user);
    }
}
