using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Commands.Debits;

public sealed class DeleteDebitOwnerCommand
    : DeleteEntityOwnerCommand<Debit, Guid, DebitPermissions>;

public sealed class DeleteDebitOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteDebitOwnerCommand, Debit, Guid, DebitPermissions>(dbContext);
