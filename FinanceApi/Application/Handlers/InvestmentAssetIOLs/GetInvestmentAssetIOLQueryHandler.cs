using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class GetInvestmentAssetIOLQueryHandler : BaseResponseHandler<GetInvestmentAssetIOLQuery, InvestmentAssetIOL>
{
    public GetInvestmentAssetIOLQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<InvestmentAssetIOL> Handle(GetInvestmentAssetIOLQuery request, CancellationToken cancellationToken)
    {
        var bank = await DbContext.InvestmentAssetIOLs.FirstOrDefaultAsync(o => o.Id == request.Id);

        if (bank == null) throw new Exception("InvestmentAssetIOL not found");

        return await Task.FromResult(bank);
    }
}
