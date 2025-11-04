using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistence;

namespace Finance.Application.Queries.AppModules;

public class GetAppModuleTypeQueryHandler : BaseQueryHandler<GetAppModuleTypeQuery, AppModuleType?>
{
    private readonly IRepository<AppModuleType, AppModuleTypeEnum> appModuleTypeRepository;

    public GetAppModuleTypeQueryHandler(
        FinanceDbContext db,
        IRepository<AppModuleType, AppModuleTypeEnum> appModuleTypeRepository)
        : base(db)
    {
        this.appModuleTypeRepository = appModuleTypeRepository;
    }

    public override async Task<DataResult<AppModuleType?>> ExecuteAsync(GetAppModuleTypeQuery request, CancellationToken cancellationToken)
        => DataResult<AppModuleType?>.Success(await appModuleTypeRepository.GetByIdAsync(request.Id, cancellationToken));
}

public class GetAppModuleTypeQuery : GetSingleByIdQuery<AppModuleType, AppModuleTypeEnum>;
