using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.AppModules;

public class GetAppModuleQueryHandler : BaseResponseHandler<GetAppModuleQuery, AppModule?>
{
    private readonly IAppModuleRepository appModuleRepository;

    public GetAppModuleQueryHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
    }

    public override async Task<AppModule?> Handle(GetAppModuleQuery request, CancellationToken cancellationToken)
        => await appModuleRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetAppModuleQuery : GetSingleByIdQuery<AppModule?, Guid>
{
}
