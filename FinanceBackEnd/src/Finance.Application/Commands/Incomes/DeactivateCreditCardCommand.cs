using Finance.Application.Commands.Base;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;

namespace Finance.Application.Commands.Incomes;

public class DeactivateIncomeCommand : BatchUpdateBaseCommand;

public class DeactivateIncomeCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateIncomeCommand, DeactivateIncomeCommandValidator, Income, Guid>(dbContext);

public class DeactivateIncomeCommandValidator : BatchUpdateBaseCommandValidator<DeactivateIncomeCommand>;
