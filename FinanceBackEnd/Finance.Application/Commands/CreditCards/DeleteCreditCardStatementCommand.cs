using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Finance.Application.Services;

namespace Finance.Application.Commands.CreditCards;

public class DeleteCreditCardStatementCommandHandler(IEntityService<CreditCardStatement, Guid> repository)
    : BaseDeleteCommandHandler<CreditCardStatement, Guid>(repository)
{
}

public class DeleteCreditCardStatementCommand : BaseDeleteCommand<Guid>
{
}

public class DeleteCreditCardStatementCommandValidator
    : BaseDeleteCommandValidator<DeleteCreditCardStatementCommand, CreditCardStatement, Guid>
{
    public DeleteCreditCardStatementCommandValidator(
        IRepository<CreditCardStatement, Guid> repository)
        : base(repository)
    {
    }
}
