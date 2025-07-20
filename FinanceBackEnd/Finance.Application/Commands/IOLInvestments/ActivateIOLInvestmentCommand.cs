using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.IOLInvestments;

public class ActivateIOLInvestmentCommandHandler : ICommandHandler<ActivateIOLInvestmentCommand, DataResult<IOLInvestment?>>
{
    private readonly IEntityService<IOLInvestment, Guid> _service;

    public ActivateIOLInvestmentCommandHandler(
        IEntityService<IOLInvestment, Guid> service)
    {
        _service = service;
    }

    public async Task<DataResult<IOLInvestment?>> ExecuteAsync(ActivateIOLInvestmentCommand request, CancellationToken cancellationToken)
        => DataResult<IOLInvestment?>.Success(await _service.SetDeactivatedAsync(request.Id, false, cancellationToken));
}

public class ActivateIOLInvestmentCommand : ICommand
{
    public Guid Id { get; set; }
}
