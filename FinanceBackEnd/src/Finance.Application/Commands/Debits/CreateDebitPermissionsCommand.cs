using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.Debits;

public class CreateDebitPermissionsCommand : CreateResourcePermissionsCommand<Debit, Guid, DebitPermissions>;

public class CreateDebitPermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateDebitPermissionsCommand, Debit, Guid, DebitPermissions>(dbContext)
{
    protected override async Task<Debit?> QuerySource(
        CreateDebitPermissionsCommand request, CancellationToken cancellationToken)
        => await DbContext.Debit
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == request.ResourceId, cancellationToken);
}
