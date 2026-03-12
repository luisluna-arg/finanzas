using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Movements;
using Finance.Persistence;

namespace Finance.Application.Commands.Movements;

public sealed class DeleteMovementOwnerCommand
    : DeleteEntityOwnerCommand<Movement, Guid, MovementPermissions>;

public sealed class DeleteMovementOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteMovementOwnerCommand, Movement, Guid, MovementPermissions>(dbContext);
