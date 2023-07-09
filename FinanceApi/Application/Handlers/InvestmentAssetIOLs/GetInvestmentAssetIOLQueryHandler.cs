using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class GetInvestmentAssetIOLQueryHandler : BaseResponseHandler<GetInvestmentAssetIOLQuery, InvestmentAssetIOL>
{
    private readonly IRepository<InvestmentAssetIOL, Guid> investmentAssetIOLRepository;

    public GetInvestmentAssetIOLQueryHandler(
        FinanceDbContext db,
        IRepository<InvestmentAssetIOL, Guid> investmentAssetIOLRepository)
        : base(db)
    {
        this.investmentAssetIOLRepository = investmentAssetIOLRepository;
    }

    public override async Task<InvestmentAssetIOL> Handle(GetInvestmentAssetIOLQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLRepository.GetById(request.Id);
}
