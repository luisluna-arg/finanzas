using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Queries.Resources;

public class GetFundOwnershipQuery : BaseGetResourceOwnershipWithIdQuery<Fund, Guid, FundResource>
{
    public GetFundOwnershipQuery(Guid id) : base(id)
    {
    }
}

public class GetFundOwnershipQueryHandler : BaseGetResourceOwnershipWithIdQueryHandler<Fund, Guid, FundResource>
{
    public GetFundOwnershipQueryHandler(FinanceDbContext dbContext) : base(dbContext)
    {
    }
}
