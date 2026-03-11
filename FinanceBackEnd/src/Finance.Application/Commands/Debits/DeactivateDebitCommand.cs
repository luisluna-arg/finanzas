using Finance.Application.Commands.Base;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Commands.Debits;

public class DeactivateDebitCommand : BatchUpdateBaseCommand;

public class DeactivateDebitCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateDebitCommand, DeactivateDebitCommandValidator, Debit, Guid>(dbContext);

public class DeactivateDebitCommandValidator : BatchUpdateBaseCommandValidator<DeactivateDebitCommand>;
