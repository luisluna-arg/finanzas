using FinanceApi.Application.Queries.Banks;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Banks;

public class GetAllBanksQueryHandler : BaseCollectionHandler<GetAllBanksQuery, Bank>
{
    private readonly IRepository<Bank, Guid> bankRepository;

    public GetAllBanksQueryHandler(FinanceDbContext db, IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
    }

    public override async Task<ICollection<Bank>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
        => await bankRepository.GetAll();
}
