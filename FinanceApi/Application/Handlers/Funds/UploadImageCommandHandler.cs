using FinanceApi.Application.Commands.Funds;
using FinanceApi.Domain;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Funds;

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

    public override async Task Handle(UploadImageCommand command, CancellationToken cancellationToken)
        => await appModuleRepository.GetFund();
}
