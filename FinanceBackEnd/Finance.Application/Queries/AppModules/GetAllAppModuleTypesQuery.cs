using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.AppModules;

public class GetAllAppModuleTypesQueryHandler : BaseCollectionHandler<GetAllAppModuleTypesQuery, AppModuleType?>
{
    private readonly IRepository<AppModuleType, short> appModuleTypeRepository;

    public GetAllAppModuleTypesQueryHandler(
        FinanceDbContext db,
        IRepository<AppModuleType, short> appModuleTypeRepository)
        : base(db)
    {
        this.appModuleTypeRepository = appModuleTypeRepository;
    }

    public override async Task<ICollection<AppModuleType?>> Handle(GetAllAppModuleTypesQuery request, CancellationToken cancellationToken)
        => await (request.IncludeDeactivated ?
            this.appModuleTypeRepository.GetAllBy(new Dictionary<string, object>() { { "Deactivated", request.IncludeDeactivated } }).ToArrayAsync() :
            this.appModuleTypeRepository.GetAllAsync(cancellationToken));
}

public class GetAllAppModuleTypesQuery : GetAllQuery<AppModuleType?>
{
    public GetAllAppModuleTypesQuery()
    {
    }
}
