using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class GetInvestmentAssetIOLTypeQueryHandler : BaseResponseHandler<GetInvestmentAssetIOLTypeQuery, InvestmentAssetIOLType>
{
    private readonly IRepository<InvestmentAssetIOLType, ushort> investmentAssetIOLTypeRepository;

    public GetInvestmentAssetIOLTypeQueryHandler(
        FinanceDbContext db,
        IRepository<InvestmentAssetIOLType, ushort> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<InvestmentAssetIOLType> Handle(GetInvestmentAssetIOLTypeQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLTypeRepository.GetById(request.Id);
}
