using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.IOLInvestments;

public class DeleteIOLInvestmentCommandHandler : ICommandHandler<DeleteIOLInvestmentCommand>
{
    private readonly IEntityService<IOLInvestment, Guid> _service;

    public DeleteIOLInvestmentCommandHandler(
        IEntityService<IOLInvestment, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteIOLInvestmentCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteIOLInvestmentCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}