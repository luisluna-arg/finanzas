using FinanceApi.Application.Queries.IOLInvestments;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.IOLInvestments;

public class GetIOLInvestmentQueryHandler : BaseResponseHandler<GetIOLInvestmentQuery, IOLInvestment>
{
    private readonly IRepository<IOLInvestment, Guid> investmentAssetIOLRepository;

    public GetIOLInvestmentQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRepository)
        : base(db)
    {
        this.investmentAssetIOLRepository = investmentAssetIOLRepository;
    }

    public override async Task<IOLInvestment> Handle(GetIOLInvestmentQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLRepository.GetById(request.Id);
}
