using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.AppModules;

public class GetAppModuleQueryHandler : BaseResponseHandler<GetAppModuleQuery, AppModule>
{
    private readonly IAppModuleRepository appModuleRepository;

    public GetAppModuleQueryHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
    }

    public override async Task<AppModule> Handle(GetAppModuleQuery request, CancellationToken cancellationToken)
        => await appModuleRepository.GetById(request.Id);
}
