
using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Commands.Users;
using Finance.Domain.Models;
using Finance.Persistance;

namespace Finance.Application.Commands;

public class UpdateUserCommand : BaseUserCommand
{
    public Guid UserId { get; set; }
}

public class UpdateUserCommandHandler(FinanceDbContext dbContext) : BaseCommandHandler<UpdateUserCommand, User>(dbContext)
{
    public override Task<DataResult<User>> ExecuteAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = DbContext.User.FirstOrDefault(u => u.Username == command.Username);
        if (user == null)
        {
            return Task.FromResult(DataResult<User>.Failure("User not found"));
        }

        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.Roles = command.Roles.Select(Role.Create).ToList();

        DbContext.SaveChanges();

        return Task.FromResult(DataResult<User>.Success(user));
    }
}
