using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.IOLInvestments;

public class GetSingleIOLInvestmentQueryHandler : BaseResponseHandler<GetSingleIOLInvestmentQuery, IOLInvestment?>
{
    private readonly IRepository<IOLInvestment, Guid> investmentAssetIOLRepository;

    public GetSingleIOLInvestmentQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRepository)
        : base(db)
    {
        this.investmentAssetIOLRepository = investmentAssetIOLRepository;
    }

    public override async Task<IOLInvestment?> Handle(GetSingleIOLInvestmentQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetSingleIOLInvestmentQuery : GetSingleByIdQuery<IOLInvestment?, Guid>
{
}
