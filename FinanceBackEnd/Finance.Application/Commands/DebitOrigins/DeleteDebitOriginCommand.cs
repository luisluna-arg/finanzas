using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models;

namespace Finance.Application.Commands.DebitOrigins;

public class DeleteDebitOriginCommandHandler : ICommandHandler<DeleteDebitOriginCommand>
{
    private readonly IEntityService<DebitOrigin, Guid> _service;

    public DeleteDebitOriginCommandHandler(
        IEntityService<DebitOrigin, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteDebitOriginCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteDebitOriginCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
