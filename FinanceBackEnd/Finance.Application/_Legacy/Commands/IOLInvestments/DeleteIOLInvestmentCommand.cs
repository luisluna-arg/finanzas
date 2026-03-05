using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Services;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Legacy.Commands.IOLInvestments;

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
