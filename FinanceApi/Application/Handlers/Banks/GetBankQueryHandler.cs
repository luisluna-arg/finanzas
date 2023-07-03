using FinanceApi.Application.Queries.Banks;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Banks;

public class GetBankQueryHandler : BaseResponseHandler<GetBankQuery, Bank>
{
    public GetBankQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Bank> Handle(GetBankQuery request, CancellationToken cancellationToken)
    {
        var bank = await DbContext.Bank.FirstOrDefaultAsync(o => o.Id == request.Id);

        if (bank == null) throw new Exception("Bank not found");

        return await Task.FromResult(bank);
    }
}
