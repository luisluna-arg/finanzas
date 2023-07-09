using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class GetAllInvestmentAssetIOLTypesQueryHandler : BaseCollectionHandler<GetAllInvestmentAssetIOLTypesQuery, InvestmentAssetIOLType>
{
    private readonly IRepository<InvestmentAssetIOLType, ushort> investmentAssetIOLTypesRepository;

    public GetAllInvestmentAssetIOLTypesQueryHandler(
        FinanceDbContext db,
        IRepository<InvestmentAssetIOLType, ushort> investmentAssetIOLTypesRepository)
        : base(db)
    {
        this.investmentAssetIOLTypesRepository = investmentAssetIOLTypesRepository;
    }

    public override async Task<ICollection<InvestmentAssetIOLType>> Handle(GetAllInvestmentAssetIOLTypesQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLTypesRepository.GetAll();
}
