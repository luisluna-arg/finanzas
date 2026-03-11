using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Application.Services;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Legacy.Commands.CreditCards;

public sealed class DeleteCreditCardStatementCommand : BaseDeleteCommand<Guid>;

public sealed class DeleteCreditCardStatementCommandHandler(IEntityService<CreditCardStatement, Guid> service)
    : BaseDeleteCommandHandler<CreditCardStatement, Guid>(service);

public sealed class DeleteCreditCardStatementCommandValidator(IRepository<CreditCardStatement, Guid> repository)
    : BaseDeleteCommandValidator<DeleteCreditCardStatementCommand, CreditCardStatement, Guid>(repository);
