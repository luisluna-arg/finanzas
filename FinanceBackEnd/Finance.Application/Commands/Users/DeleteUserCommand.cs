
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class DeleteUserCommand : ICommand<CommandResult>
{
    public Guid UserId { get; set; }
}

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, CommandResult>
{
    public FinanceDbContext DbContext { get; }

    public DeleteUserCommandHandler(FinanceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await DbContext.User.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user == null)
        {
            return CommandResult.Failure($"User with username {command.UserId} not found.");
        }

        user.Deactivated = true;

        await DbContext.SaveChangesAsync();

        return CommandResult.Success();
    }
}
