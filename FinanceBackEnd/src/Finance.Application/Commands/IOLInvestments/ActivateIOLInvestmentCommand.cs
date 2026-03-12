using Finance.Application.Commands.Base;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentCommand : BatchUpdateBaseCommand;

public class ActivateIOLInvestmentCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateIOLInvestmentCommand, ActivateIOLInvestmentCommandValidator, IOLInvestment, Guid>(dbContext);

public class ActivateIOLInvestmentCommandValidator : BatchUpdateBaseCommandValidator<ActivateIOLInvestmentCommand>;
