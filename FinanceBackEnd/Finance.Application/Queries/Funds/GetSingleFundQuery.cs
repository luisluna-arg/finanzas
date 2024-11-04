using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Funds;

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
