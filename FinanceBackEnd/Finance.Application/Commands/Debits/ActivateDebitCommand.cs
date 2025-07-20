using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.Debits;

public class ActivateDebitCommandHandler : ICommandHandler<ActivateDebitCommand, DataResult<Debit?>>
{
    private readonly IEntityService<Debit, Guid> _service;

    public ActivateDebitCommandHandler(
        IEntityService<Debit, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<Debit?>> ExecuteAsync(ActivateDebitCommand request, CancellationToken cancellationToken)
        => DataResult<Debit?>.Success(await _service.SetDeactivatedAsync(request.Id, false, cancellationToken));
}

public class ActivateDebitCommand : ICommand
{
    public Guid Id { get; set; }
}
