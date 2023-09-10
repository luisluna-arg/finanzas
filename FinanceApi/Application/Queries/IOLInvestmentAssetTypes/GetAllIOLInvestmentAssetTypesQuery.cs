using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.IOLInvestmentAssetTypes;

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

public class GetAllIOLInvestmentAssetTypesQuery : GetAllQuery<IOLInvestmentAssetType>
{
}
