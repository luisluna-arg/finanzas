using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Persistance;

namespace Finance.Application.Commands;

public class CreateFundResourceCommand : ICommand<DataResult<FundResource>>
{
    public Guid ResourceId { get; set; }
    public Guid FundId { get; set; }
}

public class CreateFundResourceCommandHandler(FinanceDbContext dbContext) : BaseCommandHandler<CreateFundResourceCommand, FundResource>(dbContext)
{
    public async override Task<DataResult<FundResource>> ExecuteAsync(CreateFundResourceCommand request, CancellationToken cancellationToken = default)
    {
        var fund = await DbContext.Fund.FindAsync(request.FundId);
        if (fund == null)
        {
            return DataResult<FundResource>.Failure("Fund not found");
        }

        var resource = await DbContext.Resource.FindAsync(request.ResourceId);
        if (resource == null)
        {
            return DataResult<FundResource>.Failure("Resource not found");
        }

        var fundResource = new FundResource()
        {
            Resource = resource,
            ResourceId = request.ResourceId,
            ResourceSource = fund,
            ResourceSourceId = fund.Id
        };
        DbContext.FundResource.Add(fundResource);

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<FundResource>.Success(fundResource);
    }
}
