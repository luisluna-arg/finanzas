using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.AppModules;

public class GetAppModuleQuery : GetSingleByIdQuery<AppModule, Guid>;

public class GetAppModuleQueryHandler(FinanceDbContext db, IAppModuleRepository repository)
    : BaseQueryHandler<GetAppModuleQuery, AppModule?>(db)
{
    private readonly IAppModuleRepository _repository = repository;

    public override async Task<DataResult<AppModule?>> ExecuteAsync(GetAppModuleQuery request, CancellationToken cancellationToken)
        => DataResult<AppModule?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
