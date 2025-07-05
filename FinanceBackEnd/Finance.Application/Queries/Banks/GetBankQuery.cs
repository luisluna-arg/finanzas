using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Banks;

public class GetBankQuery : GetSingleByIdQuery<Bank?, Guid>;

public class GetBankQueryHandler(FinanceDbContext db, IRepository<Bank, Guid> repository) : BaseResponseHandler<GetBankQuery, Bank?>(db)
{
    private readonly IRepository<Bank, Guid> _repository = repository;

    public override async Task<Bank?> Handle(GetBankQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
