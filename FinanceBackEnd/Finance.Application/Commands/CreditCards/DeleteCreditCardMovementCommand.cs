using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.CreditCards;

public class DeleteCreditCardMovementCommandHandler : ICommandHandler<DeleteCreditCardMovementCommand>
{
    private readonly IEntityService<CreditCardMovement, Guid> _service;

    public DeleteCreditCardMovementCommandHandler(
        IEntityService<CreditCardMovement, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteCreditCardMovementCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteCreditCardMovementCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
