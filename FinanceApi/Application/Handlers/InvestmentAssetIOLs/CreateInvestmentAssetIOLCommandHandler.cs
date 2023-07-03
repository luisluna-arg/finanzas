using FinanceApi.Application.Commands.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class CreateInvestmentAssetIOLCommandHandler : BaseResponseHandler<CreateInvestmentAssetIOLCommand, InvestmentAssetIOL>
{
    public CreateInvestmentAssetIOLCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<InvestmentAssetIOL> Handle(CreateInvestmentAssetIOLCommand command, CancellationToken cancellationToken)
    {
        var investmentAssetIOLType = await GetType(command.InvestmentAssetIOLTypeId);

        var newInvestmentAssetIOL = new InvestmentAssetIOL()
        {
            Asset = command.Asset,
            Alarms = command.Alarms,
            Quantity = command.Quantity,
            Assets = command.Assets,
            DailyVariation = command.DailyVariation,
            LastPrice = command.LastPrice,
            AverageBuyPrice = command.AverageBuyPrice,
            AverageReturnPercent = command.AverageReturnPercent,
            AverageReturn = command.AverageReturn,
            Valued = command.Valued,
            InvestmentAssetIOLType = investmentAssetIOLType
        };

        await DbContext.InvestmentAssetIOLs.AddAsync(newInvestmentAssetIOL);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(newInvestmentAssetIOL);
    }

    private async Task<InvestmentAssetIOLType> GetType(InvestmentAssetIOLTypeEnum investmentAssetIOLTypeId)
    {
        var result = await DbContext.InvestmentAssetIOLTypes.FirstOrDefaultAsync(x => x.Id == (int)investmentAssetIOLTypeId);

        if (result == null) throw new Exception("Investment Type not found");

        return result;
    }
}
