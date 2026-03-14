using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Services;
using Finance.Domain.Models.IOLInvestments;

namespace Finance.Application.Commands.IOLInvestments;

public sealed class DeleteIOLInvestmentAssetCommandHandler(IEntityService<IOLInvestmentAsset, Guid> service)
    : ICommandHandler<DeleteIOLInvestmentAssetCommand>
{
    public async Task<CommandResult> ExecuteAsync(DeleteIOLInvestmentAssetCommand request, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(request.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public sealed class DeleteIOLInvestmentAssetCommand : ICommand
{
    public Guid[] Ids { get; set; } = [];
}
