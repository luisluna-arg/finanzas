using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.Incomes;

public class GetSingleIncomeQueryHandler : BaseResponseHandler<GetSingleIncomeQuery, Income?>
{
    private readonly IRepository<Income, Guid> fundRepository;

    public GetSingleIncomeQueryHandler(
        FinanceDbContext db,
        IRepository<Income, Guid> fundRepository)
        : base(db)
    {
        this.fundRepository = fundRepository;
    }

    public override async Task<Income?> Handle(GetSingleIncomeQuery request, CancellationToken cancellationToken)
        => await fundRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetSingleIncomeQuery : GetSingleByIdQuery<Income?, Guid>
{
}
