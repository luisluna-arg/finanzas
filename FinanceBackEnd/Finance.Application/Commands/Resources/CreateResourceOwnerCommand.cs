using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Persistance;

namespace Finance.Application.Commands;

public class CreateResourceOwnerCommand : ICommand<DataResult<ResourceOwner>>
{
    public Guid UserId { get; set; }
    public Guid ResourceId { get; set; }
}

public class CreateResourceOwnerCommandHandler(FinanceDbContext dbContext) : BaseCommandHandler<CreateResourceOwnerCommand, ResourceOwner>(dbContext)
{
    public async override Task<DataResult<ResourceOwner>> ExecuteAsync(CreateResourceOwnerCommand request, CancellationToken cancellationToken = default)
    {
        var user = await DbContext.User.FindAsync(request.UserId);
        if (user == null)
        {
            return DataResult<ResourceOwner>.Failure("User not found");
        }

        var resource = await DbContext.Resource.FindAsync(request.ResourceId);
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
