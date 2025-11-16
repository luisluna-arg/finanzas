using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Queries.Resources;

public class GetFundOwnershipQuery(Guid id)
    : BaseGetResourcePermissionsWithIdQuery<Fund, Guid, FundPermissions>(id);

public class GetFundOwnershipQueryHandler(FinanceDbContext dbContext)
    : BaseGetResourcePermissionsWithIdQueryHandler<Fund, Guid, FundPermissions>(dbContext);
