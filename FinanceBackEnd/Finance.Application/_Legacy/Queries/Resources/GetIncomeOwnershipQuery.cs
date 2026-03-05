using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;

namespace Finance.Application.Legacy.Queries.Resources;

public class GetIncomeOwnershipQuery(Guid id)
    : BaseGetResourcePermissionsWithIdQuery<Income, Guid, IncomePermissions>(id);

public class GetIncomeOwnershipQueryHandler(FinanceDbContext dbContext)
    : BaseGetResourcePermissionsWithIdQueryHandler<Income, Guid, IncomePermissions>(dbContext);
