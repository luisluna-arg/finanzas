using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Domain.Enums;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.AppModules;

public class GetAppModuleTypeQueryHandler : BaseResponseHandler<GetAppModuleTypeQuery, AppModuleType?>
{
    private readonly IRepository<AppModuleType, AppModuleTypeEnum> appModuleTypeRepository;

    public GetAppModuleTypeQueryHandler(
        FinanceDbContext db,
        IRepository<AppModuleType, AppModuleTypeEnum> appModuleTypeRepository)
        : base(db)
    {
        this.appModuleTypeRepository = appModuleTypeRepository;
    }

    public override async Task<AppModuleType?> Handle(GetAppModuleTypeQuery request, CancellationToken cancellationToken)
        => await appModuleTypeRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetAppModuleTypeQuery : GetSingleByIdQuery<AppModuleType?, AppModuleTypeEnum>;
