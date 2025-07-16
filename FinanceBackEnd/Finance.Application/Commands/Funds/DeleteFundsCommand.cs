using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services;

namespace Finance.Application.Commands.Funds;

public class DeleteFundsCommandHandler : ICommandHandler<DeleteFundsCommand>
{
    private readonly IEntityService<Fund, Guid> _service;

    public DeleteFundsCommandHandler(
        IEntityService<Fund, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteFundsCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteFundsCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
