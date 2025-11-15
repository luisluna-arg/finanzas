using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.Debits;

public class DeleteDebitCommandHandler : ICommandHandler<DeleteDebitCommand>
{
    private readonly IEntityService<Debit, Guid> _service;

    public DeleteDebitCommandHandler(
        IEntityService<Debit, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteDebitCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteDebitCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
