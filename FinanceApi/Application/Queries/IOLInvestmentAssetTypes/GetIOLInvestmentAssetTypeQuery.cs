using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Queries.IOLInvestmentAssetTypes;

public class GetIOLInvestmentAssetTypeQueryHandler : BaseResponseHandler<GetIOLInvestmentAssetTypeQuery, IOLInvestmentAssetType?>
{
    private readonly IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository;

    public GetIOLInvestmentAssetTypeQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<IOLInvestmentAssetType?> Handle(GetIOLInvestmentAssetTypeQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLTypeRepository.GetById(request.Id);
}

public class GetIOLInvestmentAssetTypeQuery : GetSingleByIdQuery<IOLInvestmentAssetType, ushort>
{
}
