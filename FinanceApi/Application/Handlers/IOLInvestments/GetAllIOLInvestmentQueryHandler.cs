using FinanceApi.Application.Queries.IOLInvestments;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.IOLInvestments;

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
