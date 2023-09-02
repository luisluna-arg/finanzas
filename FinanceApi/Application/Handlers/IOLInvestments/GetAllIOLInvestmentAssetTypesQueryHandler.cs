using FinanceApi.Application.Queries.IOLInvestments;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.IOLInvestments;

public class GetAllIOLInvestmentAssetTypesQueryHandler : BaseCollectionHandler<GetAllIOLInvestmentAssetTypesQuery, IOLInvestmentAssetType>
{
    private readonly IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypesRepository;

    public GetAllIOLInvestmentAssetTypesQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypesRepository)
        : base(db)
    {
        this.investmentAssetIOLTypesRepository = investmentAssetIOLTypesRepository;
    }

    public override async Task<ICollection<IOLInvestmentAssetType>> Handle(GetAllIOLInvestmentAssetTypesQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLTypesRepository.GetAll();
}
