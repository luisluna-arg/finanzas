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
        => await (request.Deactivated.HasValue ?
            this.appModuleTypeRepository.GetAllBy(new Dictionary<string, object>() { { "Deactivated", request.Deactivated.Value } }).ToArrayAsync() :
            this.appModuleTypeRepository.GetAll());
}

public class GetAllAppModuleTypesQuery : GetAllQuery<AppModuleType?>
{
    public GetAllAppModuleTypesQuery(bool? deactivated = null)
    {
        this.Deactivated = deactivated;
    }

    public bool? Deactivated { get; }
}
