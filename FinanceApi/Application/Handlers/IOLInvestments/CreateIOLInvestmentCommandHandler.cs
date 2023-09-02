using FinanceApi.Application.Commands.IOLInvestments;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.IOLInvestments;

public class CreateIOLInvestmentCommandHandler : BaseResponseHandler<CreateIOLInvestmentCommand, IOLInvestment>
{
    private readonly IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLRepository;
    private readonly IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository;
    private readonly IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository;

    public CreateIOLInvestmentCommandHandler(
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

    public override async Task<IOLInvestment> Handle(CreateIOLInvestmentCommand command, CancellationToken cancellationToken)
    {
        var newInvestmentAssetIOL = new IOLInvestment()
        {
            Asset = await GetAsset(command.AssetSymbol),
            TimeStamp = DateTime.UtcNow,
            Alarms = command.Alarms,
            Quantity = command.Quantity,
            Assets = command.Assets,
            DailyVariation = command.DailyVariation,
            LastPrice = command.LastPrice,
            AverageBuyPrice = command.AverageBuyPrice,
            AverageReturnPercent = command.AverageReturnPercent,
            AverageReturn = command.AverageReturn,
            Valued = command.Valued
        };

        await investmentAssetIOLRecordRepository.Add(newInvestmentAssetIOL);

        return await Task.FromResult(newInvestmentAssetIOL);
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
