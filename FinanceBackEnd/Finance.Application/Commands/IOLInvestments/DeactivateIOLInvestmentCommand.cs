using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.IOLInvestments;

public class DeactivateIOLInvestmentCommandHandler : ICommandHandler<DeactivateIOLInvestmentCommand, DataResult<IOLInvestment?>>
{
    private readonly IEntityService<IOLInvestment, Guid> _service;

    public DeactivateIOLInvestmentCommandHandler(
        IEntityService<IOLInvestment, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<IOLInvestment?>> ExecuteAsync(DeactivateIOLInvestmentCommand request, CancellationToken cancellationToken)
        => DataResult<IOLInvestment?>.Success(await _service.SetDeactivatedAsync(request.Id, true, cancellationToken));
}

public class DeactivateIOLInvestmentCommand : ICommand<DataResult<IOLInvestment?>>
{
    public Guid Id { get; set; }
}
