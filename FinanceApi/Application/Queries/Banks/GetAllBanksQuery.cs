using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.Banks;

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

public class GetAllBanksQuery : GetAllQuery<Bank>
{
}
