using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.AppModules;

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
