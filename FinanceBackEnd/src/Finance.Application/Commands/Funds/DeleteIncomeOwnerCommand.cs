
using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;

namespace Finance.Application.Commands;

public class DeleteIncomeOwnerCommand
    : DeleteEntityOwnerCommand<Income, Guid, IncomePermissions>;

public class DeleteIncomeOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteIncomeOwnerCommand, Income, Guid, IncomePermissions>(dbContext);
