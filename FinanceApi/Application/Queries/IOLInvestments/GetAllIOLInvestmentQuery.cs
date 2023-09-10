using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.IOLInvestments;

public class GetAllIOLInvestmentQueryHandler : BaseCollectionHandler<GetAllIOLInvestmentsQuery, IOLInvestment>
{
    private readonly IRepository<IOLInvestment, Guid> repository;

    public GetAllIOLInvestmentQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRepository)
        : base(db)
    {
        repository = investmentAssetIOLRepository;
    }

    public override async Task<ICollection<IOLInvestment>> Handle(GetAllIOLInvestmentsQuery request, CancellationToken cancellationToken)
        => await repository.GetAll();
}

public class GetAllIOLInvestmentsQuery : GetAllQuery<IOLInvestment>
{
}
