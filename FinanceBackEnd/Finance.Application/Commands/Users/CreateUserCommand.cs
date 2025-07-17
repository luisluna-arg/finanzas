using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Commands.Users;
using Finance.Domain.Models;
using Finance.Persistance;

namespace Finance.Application.Commands;

public class CreateUserCommand : BaseUserCommand;

public class CreateUserCommandHandler(FinanceDbContext dbContext) : BaseCommandHandler<CreateUserCommand, User>(dbContext)
{
    public async override Task<DataResult<User>> ExecuteAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Roles = request.Roles.Select(r => Role.Create(r)).ToList()
        };
        DbContext.User.Add(user);

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<User>.Success(user);
    }
}
