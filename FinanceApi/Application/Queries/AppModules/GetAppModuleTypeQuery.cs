using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.AppModules;

public class GetAppModuleTypeQueryHandler : BaseResponseHandler<GetAppModuleTypeQuery, AppModuleType?>
{
    private readonly IRepository<AppModuleType, short> appModuleTypeRepository;

    public GetAppModuleTypeQueryHandler(
        FinanceDbContext db,
        IRepository<AppModuleType, short> appModuleTypeRepository)
        : base(db)
    {
        this.appModuleTypeRepository = appModuleTypeRepository;
    }

    public override async Task<AppModuleType?> Handle(GetAppModuleTypeQuery request, CancellationToken cancellationToken)
        => await appModuleTypeRepository.GetById(request.Id);
}

public class GetAppModuleTypeQuery : GetSingleByIdQuery<AppModuleType?, short>
{
}
