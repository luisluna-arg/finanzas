using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public abstract class CreateResourcePermissionsCommand<TResource, TId, TResourcePermissions>
    : IContextAwareCommand<FinanceDispatchContext, DataResult<TResourcePermissions>>
    where TResource : Entity<TId>, new()
    where TResourcePermissions : ResourcePermissions<TResource, TId>, new()
{
    public required TId ResourceId { get; set; }
    public Guid? UserId { get; set; }
    public required ICollection<PermissionLevelEnum> PermissionLevels { get; set; }
    public FinanceDispatchContext Context { get; set; } = new();

    public void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}

public abstract class CreateResourcePermissionsCommandHandler<TCommand, TResource, TId, TResourcePermissions>(FinanceDbContext dbContext)
    : BaseCommandHandler<TCommand, TResourcePermissions>(dbContext)
    where TResource : Entity<TId>, new()
    where TResourcePermissions : ResourcePermissions<TResource, TId>, new()
    where TCommand : CreateResourcePermissionsCommand<TResource, TId, TResourcePermissions>
{
    public async override Task<DataResult<TResourcePermissions>> ExecuteAsync(TCommand request, CancellationToken cancellationToken = default)
    {
        var resource = await QuerySource(request, cancellationToken);
        if (resource == null)
        {
            return DataResult<TResourcePermissions>.Failure("Resource not found");
        }

        User? user = null;
        if (request.UserId.HasValue)
        {
            user = await DbContext.User
            .Include(u => u.Identities)
            .FirstOrDefaultAsync(u => u.Id == request.UserId.Value, cancellationToken);
        }
        else
        {
            user = await DbContext.User
                .Include(u => u.Identities)
                .FirstOrDefaultAsync(u => u.Identities.Any(i => i.SourceId == request.Context.UserIdClaim), cancellationToken);
        }

        if (user == null)
        {
            return DataResult<TResourcePermissions>.Failure("User not found");
        }

        var FundPermissions = new TResourcePermissions()
        {
            Resource = resource,
            ResourceId = request.ResourceId,
            User = user,
            UserId = user.Id,
            PermissionLevels = request.PermissionLevels
        };
        DbContext.Set<TResourcePermissions>().Add(FundPermissions);

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<TResourcePermissions>.Success(FundPermissions);
    }

    protected abstract Task<TResource?> QuerySource(TCommand request, CancellationToken cancellationToken);
}
