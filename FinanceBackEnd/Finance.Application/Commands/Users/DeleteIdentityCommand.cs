
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class DeleteIdentityCommand : ICommand<CommandResult>
{
    public Guid UserId { get; set; }
    public Guid IdentityId { get; set; }
}

public class DeleteIdentityCommandHandler : ICommandHandler<DeleteIdentityCommand, CommandResult>
{
    public FinanceDbContext DbContext { get; }

    public DeleteIdentityCommandHandler(FinanceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteIdentityCommand command, CancellationToken cancellationToken = default)
    {
        var identity = await DbContext.Identity
            .FirstOrDefaultAsync(i => i.Id == command.IdentityId && i.UserId == command.UserId, cancellationToken);

        if (identity == null)
        {
            return CommandResult.Failure("Identity not found.");
        }

        DbContext.Identity.Remove(identity);
        await DbContext.SaveChangesAsync(cancellationToken);

        return CommandResult.Success();
    }
}
