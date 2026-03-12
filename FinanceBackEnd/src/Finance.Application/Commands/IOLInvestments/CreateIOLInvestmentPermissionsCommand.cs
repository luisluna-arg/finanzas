using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.IOLInvestments;

public class CreateIOLInvestmentPermissionsCommand : CreateResourcePermissionsCommand<IOLInvestment, Guid, IOLInvestmentPermissions>;

public class CreateIOLInvestmentPermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateIOLInvestmentPermissionsCommand, IOLInvestment, Guid, IOLInvestmentPermissions>(dbContext)
{
    protected override async Task<IOLInvestment?> QuerySource(
        CreateIOLInvestmentPermissionsCommand request, CancellationToken cancellationToken)
        => await DbContext.IOLInvestment
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Id == request.ResourceId, cancellationToken);
}
