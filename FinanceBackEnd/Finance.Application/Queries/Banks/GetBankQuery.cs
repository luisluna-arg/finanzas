using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Banks;

public class GetBankQueryHandler : BaseResponseHandler<GetBankQuery, Bank?>
{
    private readonly IRepository<Bank, Guid> bankRepository;

    public GetBankQueryHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
    }

    public override async Task<Bank?> Handle(GetBankQuery request, CancellationToken cancellationToken)
        => await bankRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetBankQuery : GetSingleByIdQuery<Bank?, Guid>
{
}
