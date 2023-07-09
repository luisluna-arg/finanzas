using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class GetAllInvestmentAssetIOLsQueryHandler : BaseCollectionHandler<GetAllInvestmentAssetIOLsQuery, InvestmentAssetIOL>
{
    private IRepository<InvestmentAssetIOL, Guid> investmentAssetIOLRepository;

    public GetAllInvestmentAssetIOLsQueryHandler(
        FinanceDbContext db,
        IRepository<InvestmentAssetIOL, Guid> investmentAssetIOLRepository)
        : base(db)
    {
    }

    public override async Task<ICollection<InvestmentAssetIOL>> Handle(GetAllInvestmentAssetIOLsQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLRepository.GetAll();
}
