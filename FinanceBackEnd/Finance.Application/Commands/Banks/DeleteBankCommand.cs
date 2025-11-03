using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models;

namespace Finance.Application.Commands.Banks;

public class DeleteBankCommandHandler : ICommandHandler<DeleteBankCommand>
{
    private readonly IEntityService<Bank, Guid> _service;

    public DeleteBankCommandHandler(
        IEntityService<Bank, Guid> service)
    {
        _service = service;
    }

    public async Task<CommandResult> ExecuteAsync(DeleteBankCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class DeleteBankCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
