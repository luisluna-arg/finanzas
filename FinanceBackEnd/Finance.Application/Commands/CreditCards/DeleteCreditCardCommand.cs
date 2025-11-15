using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Commands.CreditCards;

public class DeleteCreditCardCommandHandler : ICommandHandler<DeleteCreditCardCommand>
{
    private readonly IEntityService<CreditCard, Guid> _service;

    public DeleteCreditCardCommandHandler(
        IEntityService<CreditCard, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteCreditCardCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteCreditCardCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
