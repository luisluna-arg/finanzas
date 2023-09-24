using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.AppModules;

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
        => await appModuleRepository.GetById(request.Id);
}

public class GetAppModuleQuery : GetSingleByIdQuery<AppModule?, Guid>
{
}
