using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.IOLInvestments;

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
