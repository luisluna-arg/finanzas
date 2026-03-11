using Finance.Application.Commands.Base;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Commands.Funds;

public class DeactivateFundCommand : BatchUpdateBaseCommand;

public class DeactivateFundCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateFundCommand, DeactivateFundCommandValidator, Fund, Guid>(dbContext);

public class DeactivateFundCommandValidator : BatchUpdateBaseCommandValidator<DeactivateFundCommand>;
