using Finance.Application.Commands.Base;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Commands.Funds;

public class ActivateFundCommand : BatchUpdateBaseCommand;

public class ActivateFundCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateFundCommand, ActivateFundCommandValidator, Fund, Guid>(dbContext);

public class ActivateFundCommandValidator : BatchUpdateBaseCommandValidator<ActivateFundCommand>;
