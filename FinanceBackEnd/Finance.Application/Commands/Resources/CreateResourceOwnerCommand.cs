using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.Auth;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class CreateResourceOwnerCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<ResourceOwner>>
{
    public Guid ResourceId { get; set; }
    public FinanceDispatchContext Context { get; set; } = new();

    public void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}

public class CreateResourceOwnerCommandHandler(FinanceDbContext dbContext) : BaseCommandHandler<CreateResourceOwnerCommand, ResourceOwner>(dbContext)
{
    public async override Task<DataResult<ResourceOwner>> ExecuteAsync(CreateResourceOwnerCommand request, CancellationToken cancellationToken = default)
    {
        var user = await DbContext.User
            .Include(u => u.Identities)
            .FirstOrDefaultAsync(u => u.Identities.Any(i => i.SourceId == request.Context.UserIdClaim), cancellationToken);
        if (user == null)
        {
            return DataResult<ResourceOwner>.Failure("User not found");
        }

        var resource = await DbContext.Resource.FirstOrDefaultAsync(r => r.Id == request.ResourceId, cancellationToken);
        if (resource == null)
        {
            return DataResult<ResourceOwner>.Failure("Resource not found");
        }

        var resourceOwner = new ResourceOwner()
        {
            User = user,
            Resource = resource
        };
        DbContext.ResourceOwner.Add(resourceOwner);

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<ResourceOwner>.Success(resourceOwner);
    }
}
