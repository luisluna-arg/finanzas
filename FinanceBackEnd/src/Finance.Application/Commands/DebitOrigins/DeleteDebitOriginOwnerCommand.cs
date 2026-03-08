using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Commands.DebitOrigins;

public class DeleteDebitOriginOwnerCommand
    : DeleteEntityOwnerCommand<DebitOrigin, Guid, DebitOriginPermissions>;

public class DeleteDebitOriginOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteDebitOriginOwnerCommand, DebitOrigin, Guid, DebitOriginPermissions>(dbContext);
