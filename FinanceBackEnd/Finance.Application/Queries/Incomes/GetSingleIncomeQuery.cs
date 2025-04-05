using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Incomes;

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
