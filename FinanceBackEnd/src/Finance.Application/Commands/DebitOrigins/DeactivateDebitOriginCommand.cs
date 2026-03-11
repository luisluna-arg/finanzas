using Finance.Application.Commands.Base;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Commands.DebitOrigins;

public class DeactivateDebitOriginCommand : BatchUpdateBaseCommand;

public class DeactivateDebitOriginCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateDebitOriginCommand, DeactivateDebitOriginCommandValidator, DebitOrigin, Guid>(dbContext);

public class DeactivateDebitOriginCommandValidator : BatchUpdateBaseCommandValidator<DeactivateDebitOriginCommand>;
