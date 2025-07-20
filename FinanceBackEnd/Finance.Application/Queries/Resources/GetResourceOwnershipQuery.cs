using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Resources;

public class GetResourceOwnershipQuery : IQuery<List<(Resource Resource, ResourceOwner ResourceOwner, FundResource FundResource)>>
{
    public Guid UserId { get; }
    public Guid FundId { get; }

    public GetResourceOwnershipQuery(Guid userId, Guid fundId)
    {
        UserId = userId;
        FundId = fundId;
    }
}

public class GetResourceOwnershipQueryHandler : IQueryHandler<GetResourceOwnershipQuery, List<(Resource Resource, ResourceOwner ResourceOwner, FundResource FundResource)>>
{
    public FinanceDbContext DbContext { get; }

    public GetResourceOwnershipQueryHandler(FinanceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<DataResult<List<(Resource Resource, ResourceOwner ResourceOwner, FundResource FundResource)>>> ExecuteAsync(GetResourceOwnershipQuery request, CancellationToken cancellationToken = default)
    {
        var query =
            from r in DbContext.Resource
            join o in DbContext.ResourceOwner.Include(ro => ro.User).Where(ro => ro.UserId == request.UserId)
                on r.Id equals o.ResourceId
            join fr in DbContext.FundResource.Include(ro => ro.ResourceSource).Where(fr => fr.ResourceSourceId == request.FundId)
                on o.ResourceId equals fr.ResourceId
            select new
            {
                Resource = r,
                ResourceOwner = o,
                FundResource = fr
            };

        var results = await query.ToListAsync(cancellationToken);

        var aa = results.Select(d => (d.Resource, d.ResourceOwner, d.FundResource)).ToList();

        return DataResult<List<(Resource Resource, ResourceOwner ResourceOwner, FundResource FundResource)>>.Success(aa);
    }
}