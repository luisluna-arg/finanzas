using Finance.Application.Commands.Base;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;

namespace Finance.Application.Commands.Incomes;

public class ActivateIncomeCommand : BatchUpdateBaseCommand;

public class ActivateIncomeCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateIncomeCommand, ActivateIncomeCommandValidator, Income, Guid>(dbContext);

public class ActivateIncomeCommandValidator : BatchUpdateBaseCommandValidator<ActivateIncomeCommand>;
