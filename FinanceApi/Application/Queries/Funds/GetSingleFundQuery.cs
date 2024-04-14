using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.Funds;

public class GetSingleFundQueryHandler : BaseResponseHandler<GetSingleFundQuery, Fund?>
{
    private readonly IRepository<Fund, Guid> fundRepository;

    public GetSingleFundQueryHandler(
        FinanceDbContext db,
        IRepository<Fund, Guid> fundRepository)
        : base(db)
    {
        this.fundRepository = fundRepository;
    }

    public override async Task<Fund?> Handle(GetSingleFundQuery request, CancellationToken cancellationToken)
        => await fundRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetSingleFundQuery : GetSingleByIdQuery<Fund?, Guid>
{
}
