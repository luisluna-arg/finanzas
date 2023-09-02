using FinanceApi.Application.Queries.IOLInvestments;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.IOLInvestments;

public class GetIOLInvestmentAssetTypeQueryHandler : BaseResponseHandler<GetIOLInvestmentAssetTypeQuery, IOLInvestmentAssetType>
{
    private readonly IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository;

    public GetIOLInvestmentAssetTypeQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<IOLInvestmentAssetType> Handle(GetIOLInvestmentAssetTypeQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLTypeRepository.GetById(request.Id);
}
