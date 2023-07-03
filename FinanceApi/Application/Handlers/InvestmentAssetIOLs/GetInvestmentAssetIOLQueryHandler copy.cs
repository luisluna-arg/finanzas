using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class GetInvestmentAssetIOLTypeQueryHandler : BaseResponseHandler<GetInvestmentAssetIOLTypeQuery, InvestmentAssetIOLType>
{
    public GetInvestmentAssetIOLTypeQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<InvestmentAssetIOLType> Handle(GetInvestmentAssetIOLTypeQuery request, CancellationToken cancellationToken)
    {
        var record = await DbContext.InvestmentAssetIOLTypes.FirstOrDefaultAsync(o => o.Id == request.Id);

        if (record == null) throw new Exception("Investment Asset IOL Type not found");

        return await Task.FromResult(record);
    }
}
