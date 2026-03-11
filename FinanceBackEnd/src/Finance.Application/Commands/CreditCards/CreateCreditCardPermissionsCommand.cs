using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.CreditCards;

public class CreateCreditCardPermissionsCommand : CreateResourcePermissionsCommand<CreditCard, Guid, CreditCardPermissions>;

public class CreateCreditCardPermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateCreditCardPermissionsCommand, CreditCard, Guid, CreditCardPermissions>(dbContext)
{
    protected override async Task<CreditCard?> QuerySource(
        CreateCreditCardPermissionsCommand request, CancellationToken cancellationToken)
        => await DbContext.CreditCard
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.ResourceId, cancellationToken);
}
