using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Movements;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.Movements;

public class CreateMovementPermissionsCommand : CreateResourcePermissionsCommand<Movement, Guid, MovementPermissions>;

public class CreateMovementPermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateMovementPermissionsCommand, Movement, Guid, MovementPermissions>(dbContext)
{
    protected override async Task<Movement?> QuerySource(
        CreateMovementPermissionsCommand request, CancellationToken cancellationToken)
        => await DbContext.Movement
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == request.ResourceId, cancellationToken);
}
