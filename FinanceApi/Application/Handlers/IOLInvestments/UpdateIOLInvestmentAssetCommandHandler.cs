using FinanceApi.Application.Commands.IOLInvestments;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.IOLInvestments;

public class UpdateIOLInvestmentAssetCommandHandler : BaseResponseHandler<UpdateIOLInvestmentCommand, IOLInvestment>
{
    private readonly IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLRepository;
    private readonly IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository;
    private readonly IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository;

    public UpdateIOLInvestmentAssetCommandHandler(
        FinanceDbContext db,
        IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLRepository,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository,
        IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.investmentAssetIOLRepository = investmentAssetIOLRepository;
        this.investmentAssetIOLRecordRepository = investmentAssetIOLRecordRepository;
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<IOLInvestment> Handle(UpdateIOLInvestmentCommand command, CancellationToken cancellationToken)
    {
        var investmentAssetIOL = await investmentAssetIOLRecordRepository.GetById(command.Id);

        investmentAssetIOL.Asset = await GetAsset(command.AssetSymbol);
        investmentAssetIOL.Alarms = command.Alarms;
        investmentAssetIOL.Quantity = command.Quantity;
        investmentAssetIOL.Assets = command.Assets;
        investmentAssetIOL.DailyVariation = command.DailyVariation;
        investmentAssetIOL.LastPrice = command.LastPrice;
        investmentAssetIOL.AverageBuyPrice = command.AverageBuyPrice;
        investmentAssetIOL.AverageReturnPercent = command.AverageReturnPercent;
        investmentAssetIOL.AverageReturn = command.AverageReturn;
        investmentAssetIOL.Valued = command.Valued;

        await investmentAssetIOLRecordRepository.Update(investmentAssetIOL);

        return await Task.FromResult(investmentAssetIOL);
    }

    private async Task<IOLInvestmentAsset> GetAsset(string assetSymbol)
    {
        var asset = await investmentAssetIOLRepository.GetBy("Symbol", assetSymbol);

        if (asset == null)
        {
            var assetType = await investmentAssetIOLTypeRepository.GetBy("Name", IOLInvestmentAssetType.Default);

            if (assetType == null)
            {
                assetType = new IOLInvestmentAssetType()
                {
                    Name = IOLInvestmentAssetType.Default
                };
            }

            asset = new IOLInvestmentAsset()
            {
                Symbol = assetSymbol,
                Description = assetSymbol,
                Type = assetType
            };
        }

        return asset!;
    }
}
