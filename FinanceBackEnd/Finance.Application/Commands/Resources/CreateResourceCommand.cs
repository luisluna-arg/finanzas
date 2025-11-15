using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Commands;

public class CreateResourceCommand : ICommand<DataResult<Resource>>;

public class CreateResourceCommandHandler(FinanceDbContext dbContext) : BaseCommandHandler<CreateResourceCommand, Resource>(dbContext)
{
    public async override Task<DataResult<Resource>> ExecuteAsync(CreateResourceCommand request, CancellationToken cancellationToken = default)
    {
        var resource = new Resource();
        DbContext.Resource.Add(resource);

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<Resource>.Success(resource);
    }
}
