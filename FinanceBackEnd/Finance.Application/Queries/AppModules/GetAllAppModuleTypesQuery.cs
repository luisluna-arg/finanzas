using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models.AppModules;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.AppModules;

public class GetAllAppModuleTypesQueryHandler : BaseCollectionHandler<GetAllAppModuleTypesQuery, AppModuleType>
{
    private readonly IRepository<AppModuleType, AppModuleTypeEnum> _appModuleTypeRepository;

    public GetAllAppModuleTypesQueryHandler(
        FinanceDbContext db,
        IRepository<AppModuleType, AppModuleTypeEnum> appModuleTypeRepository)
        : base(db)
    {
        _appModuleTypeRepository = appModuleTypeRepository;
    }

    public override async Task<DataResult<List<AppModuleType>>> ExecuteAsync(GetAllAppModuleTypesQuery request, CancellationToken cancellationToken)
    {
        List<AppModuleType> result;
        if (request.IncludeDeactivated)
        {
            result = await _appModuleTypeRepository
                .GetAllBy(new Dictionary<string, object> { { "Deactivated", request.IncludeDeactivated } })
                .ToListAsync(cancellationToken);
        }
        else
        {
            result = (await _appModuleTypeRepository.GetAllAsync(cancellationToken)).ToList();
        }

        return DataResult<List<AppModuleType>>.Success(result);
    }
}

public class GetAllAppModuleTypesQuery() : GetAllQuery<AppModuleType>();
