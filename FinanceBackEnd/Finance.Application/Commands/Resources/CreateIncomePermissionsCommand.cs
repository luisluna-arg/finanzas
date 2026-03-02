using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class CreateIncomePermissionsCommand : CreateResourcePermissionsCommand<Income, Guid, IncomePermissions>;

public class CreateIncomePermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateIncomePermissionsCommand, Income, Guid, IncomePermissions>(dbContext)
{
    protected override async Task<Income?> QuerySource(CreateIncomePermissionsCommand request, CancellationToken cancellationToken)
    {
        return await DbContext.Income
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.Id == request.ResourceId, cancellationToken);
    }
}
