using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Application.Services;

namespace Finance.Application.Commands.CreditCards;

public class DeleteCreditCardStatementCommand : BaseDeleteCommand<Guid>;

public class DeleteCreditCardStatementCommandHandler(IEntityService<CreditCardStatement, Guid> service)
    : BaseDeleteCommandHandler<CreditCardStatement, Guid>(service);

public class DeleteCreditCardStatementCommandValidator(IRepository<CreditCardStatement, Guid> repository)
    : BaseDeleteCommandValidator<DeleteCreditCardStatementCommand, CreditCardStatement, Guid>(repository);