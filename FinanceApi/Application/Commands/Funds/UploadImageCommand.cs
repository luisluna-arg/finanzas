using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Funds;

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
        => await appModuleRepository.GetFundsAsync(cancellationToken);
}

public class UploadImageCommand : IRequest
{
    required public IFormFileCollection Files { get; set; }

    public DateTimeKind? DateKind { get; set; }

    public DateTime? DateReference { get; internal set; }
}
