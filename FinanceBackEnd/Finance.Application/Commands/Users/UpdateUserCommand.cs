
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

public class UpdateUserCommandHandler : BaseCommandHandler<UpdateUserCommand, User>
{
    public UpdateUserCommandHandler(FinanceDbContext dbContext) : base(dbContext) { }

    public override Task<DataResult<User>> ExecuteAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = DbContext.User.FirstOrDefault(u => u.Username == command.Username);
        if (user == null)
        {
            return Task.FromResult(DataResult<User>.Failure("User not found"));
        }

        var roles = DbContext.Role
            .Where(r => command.Roles.Contains(r.Id))
            .ToList();

        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.Roles = roles;

        DbContext.SaveChanges();

        return Task.FromResult(DataResult<User>.Success(user));
    }
}
