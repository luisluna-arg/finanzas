using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

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
        var fund = await DbContext.Fund
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.Id == request.FundId, cancellationToken);
        if (fund == null)
        {
            return DataResult<FundResource>.Failure("Fund not found");
        }

        var resource = await DbContext.Resource
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == request.ResourceId, cancellationToken);
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
