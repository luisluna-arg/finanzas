using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.Debits;

public class DeactivateDebitCommandHandler : ICommandHandler<DeactivateDebitCommand, DataResult<Debit?>>
{
    private readonly IEntityService<Debit, Guid> _service;

    public DeactivateDebitCommandHandler(
        IEntityService<Debit, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<Debit?>> ExecuteAsync(DeactivateDebitCommand request, CancellationToken cancellationToken)
        => DataResult<Debit?>.Success(await _service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}

public class DeactivateDebitCommand : ICommand<DataResult<Debit?>>
{
    public Guid Id { get; set; }
}
