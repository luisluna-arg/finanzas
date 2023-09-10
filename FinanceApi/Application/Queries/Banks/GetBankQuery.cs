using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.Banks;

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
        => await bankRepository.GetById(request.Id);
}

public class GetBankQuery : GetSingleByIdQuery<Bank, Guid>
{
}
