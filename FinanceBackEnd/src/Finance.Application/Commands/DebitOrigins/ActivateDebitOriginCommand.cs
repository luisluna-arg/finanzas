using Finance.Application.Commands.Base;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Commands.DebitOrigins;

public class ActivateDebitOriginCommand : BatchUpdateBaseCommand;

public class ActivateDebitOriginCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateDebitOriginCommand, ActivateDebitOriginCommandValidator, DebitOrigin, Guid>(dbContext);

public class ActivateDebitOriginCommandValidator : BatchUpdateBaseCommandValidator<ActivateDebitOriginCommand>;
