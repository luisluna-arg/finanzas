using FinanceApi.Application.Queries.Banks;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Banks;

public class GetBankQueryHandler : BaseResponseHandler<GetBankQuery, Bank>
{
    private readonly IRepository<Bank, Guid> bankRepository;

    public GetBankQueryHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
    }

    public override async Task<Bank> Handle(GetBankQuery request, CancellationToken cancellationToken)
        => await bankRepository.GetById(request.Id);
}
