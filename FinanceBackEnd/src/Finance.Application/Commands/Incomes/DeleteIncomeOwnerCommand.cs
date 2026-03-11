
using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;

namespace Finance.Application.Commands.Incomes;

public sealed class DeleteIncomeOwnerCommand
    : DeleteEntityOwnerCommand<Income, Guid, IncomePermissions>;

public sealed class DeleteIncomeOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteIncomeOwnerCommand, Income, Guid, IncomePermissions>(dbContext);
