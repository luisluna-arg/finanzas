using Finance.Application.Commands.Base;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public class DeactivateIOLInvestmentCommand : BatchUpdateBaseCommand;

public class DeactivateIOLInvestmentCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateIOLInvestmentCommand, DeactivateIOLInvestmentCommandValidator, IOLInvestment, Guid>(dbContext);

public class DeactivateIOLInvestmentCommandValidator : BatchUpdateBaseCommandValidator<DeactivateIOLInvestmentCommand>;
