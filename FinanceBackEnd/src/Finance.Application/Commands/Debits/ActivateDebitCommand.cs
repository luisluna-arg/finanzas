using Finance.Application.Commands.Base;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Commands.Debits;

public class ActivateDebitCommand : BatchUpdateBaseCommand;

public class ActivateDebitCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateDebitCommand, ActivateDebitCommandValidator, Debit, Guid>(dbContext);

public class ActivateDebitCommandValidator : BatchUpdateBaseCommandValidator<ActivateDebitCommand>;
