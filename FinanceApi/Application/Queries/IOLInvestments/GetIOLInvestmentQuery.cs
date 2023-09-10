using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.IOLInvestments;

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

public class GetIOLInvestmentQuery : GetSingleByIdQuery<IOLInvestment, Guid>
{
}
