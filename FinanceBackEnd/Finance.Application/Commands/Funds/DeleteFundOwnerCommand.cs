
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Users;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class DeleteFundOwnerCommand : OwnerBaseCommand<CommandResult>
{
    public Guid FundId { get; set; }
}

public class DeleteFundOwnerCommandHandler(FinanceDbContext dbContext) : ICommandHandler<DeleteFundOwnerCommand, CommandResult>
{
    private FinanceDbContext DbContext { get; } = dbContext;

    public async Task<CommandResult> ExecuteAsync(DeleteFundOwnerCommand command, CancellationToken cancellationToken = default)
    {
        var query =
            from r in DbContext.Resource
            join o in DbContext.ResourceOwner.Where(ro => ro.UserId == command.Context.UserInfo.Id)
                on r.Id equals o.ResourceId
            join fr in DbContext.FundResource.Where(fr => fr.ResourceSourceId == command.FundId)
                on o.ResourceId equals fr.ResourceId
            select new
            {
                Resource = r,
                ResourceOwner = o,
                FundResource = fr
            };

        var results = await query.ToListAsync(cancellationToken);

        foreach (var result in results)
        {
            DbContext.ResourceOwner.Remove(result.ResourceOwner);
            DbContext.FundResource.Remove(result.FundResource);
            DbContext.Resource.Remove(result.Resource);
        }

        await DbContext.SaveChangesAsync();

        return CommandResult.Success();
    }
}
