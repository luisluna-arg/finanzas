using FinanceApi.Application.Commands.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class UpdateInvestmentAssetIOLCommandHandler : BaseResponseHandler<UpdateInvestmentAssetIOLCommand, InvestmentAssetIOL>
{
    public UpdateInvestmentAssetIOLCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<InvestmentAssetIOL> Handle(UpdateInvestmentAssetIOLCommand command, CancellationToken cancellationToken)
    {
        var investmentAssetIOL = await GetRecord(command.Id);

        var investmentAssetIOLType = await GetType(command.InvestmentAssetIOLTypeId);

        investmentAssetIOL.Asset = command.Asset;
        investmentAssetIOL.Alarms = command.Alarms;
        investmentAssetIOL.Quantity = command.Quantity;
        investmentAssetIOL.Assets = command.Assets;
        investmentAssetIOL.DailyVariation = command.DailyVariation;
        investmentAssetIOL.LastPrice = command.LastPrice;
        investmentAssetIOL.AverageBuyPrice = command.AverageBuyPrice;
        investmentAssetIOL.AverageReturnPercent = command.AverageReturnPercent;
        investmentAssetIOL.AverageReturn = command.AverageReturn;
        investmentAssetIOL.Valued = command.Valued;
        investmentAssetIOL.InvestmentAssetIOLType = investmentAssetIOLType;

        await DbContext.SaveChangesAsync();

        return await Task.FromResult(investmentAssetIOL);
    }

    private async Task<InvestmentAssetIOL> GetRecord(Guid id)
    {
        var result = await DbContext.InvestmentAssetIOLs.FirstOrDefaultAsync(x => x.Id == id);

        if (result == null) throw new Exception("Investment record not found");

        return result;
    }

    private async Task<InvestmentAssetIOLType> GetType(InvestmentAssetIOLTypeEnum investmentAssetIOLTypeId)
    {
        var result = await DbContext.InvestmentAssetIOLTypes.FirstOrDefaultAsync(x => x.Id == (int)investmentAssetIOLTypeId);

        if (result == null) throw new Exception("Investment Type not found");

        return result;
    }
}
