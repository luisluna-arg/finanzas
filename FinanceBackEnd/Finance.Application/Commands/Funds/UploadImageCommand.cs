using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.Funds;

public class UploadImageCommandHandler : BaseResponselessHandler<UploadImageCommand>
{
    private readonly IAppModuleRepository appModuleRepository;

    public UploadImageCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
    }

    public override async Task<CommandResult> ExecuteAsync(UploadImageCommand command, CancellationToken cancellationToken)
    {
        await appModuleRepository.GetFundsAsync(cancellationToken);
        return CommandResult.Success();
    }
}

public class UploadImageCommand : ICommand
{
    required public IFormFileCollection Files { get; set; }
    public DateTimeKind? DateKind { get; set; }
    public DateTime? DateReference { get; internal set; }
}
