using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Queries.Banks;

public class GetBankQuery : GetSingleByIdQuery<Bank?, Guid>;

public class GetBankQueryHandler(FinanceDbContext db, IRepository<Bank, Guid> repository) : BaseQueryHandler<GetBankQuery, Bank?>(db)
{
    private readonly IRepository<Bank, Guid> _repository = repository;

    public override async Task<DataResult<Bank?>> ExecuteAsync(GetBankQuery request, CancellationToken cancellationToken)
        => DataResult<Bank?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
