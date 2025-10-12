using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public class UpdateIOLInvestmentCommandHandler : BaseCommandHandler<UpdateIOLInvestmentCommand, IOLInvestment>
{
    private readonly IRepository<IOLInvestmentAsset, Guid> _iolInvestmentAssetRepository;
    private readonly IRepository<IOLInvestment, Guid> _iolInvestmentRepository;
    private readonly IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> _iolInvestmentAssetTypeRepository;

    public UpdateIOLInvestmentCommandHandler(
        FinanceDbContext db,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository,
        IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLRepository,
        IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> investmentAssetIOLTypeRepository)
        : base(db)
    {
        _iolInvestmentAssetRepository = investmentAssetIOLRepository;
        _iolInvestmentRepository = investmentAssetIOLRecordRepository;
        _iolInvestmentAssetTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<DataResult<IOLInvestment>> ExecuteAsync(UpdateIOLInvestmentCommand command, CancellationToken cancellationToken)
    {
        var iolInvestmentAsset = await _iolInvestmentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (iolInvestmentAsset == null) throw new Exception("IOL Investment not found");

        if (!string.IsNullOrWhiteSpace(command.AssetSymbol))
        {
            iolInvestmentAsset.Asset = await GetAssetAsync(command.AssetSymbol, cancellationToken);
            if (iolInvestmentAsset == null) throw new Exception("IOL Investment Asset not found by symbol");
        }

        iolInvestmentAsset.Alarms = command.Alarms;
        iolInvestmentAsset.Quantity = command.Quantity;
        iolInvestmentAsset.Assets = command.Assets;
        iolInvestmentAsset.DailyVariation = command.DailyVariation;
        iolInvestmentAsset.LastPrice = command.LastPrice;
        iolInvestmentAsset.AverageBuyPrice = command.AverageBuyPrice;
        iolInvestmentAsset.AverageReturnPercent = command.AverageReturnPercent;
        iolInvestmentAsset.AverageReturn = command.AverageReturn;
        iolInvestmentAsset.Valued = command.Valued;

        await _iolInvestmentRepository.UpdateAsync(iolInvestmentAsset, cancellationToken);

        return DataResult<IOLInvestment>.Success(iolInvestmentAsset);
    }

    private async Task<IOLInvestmentAsset> GetAssetAsync(string assetSymbol, CancellationToken cancellationToken)
    {
        var asset = await _iolInvestmentAssetRepository.GetByAsync("Symbol", assetSymbol, cancellationToken);

        if (asset == null)
        {
            var assetType = await _iolInvestmentAssetTypeRepository.GetByAsync("Name", IOLInvestmentAssetType.DefaultName, cancellationToken);

            if (assetType == null)
            {
                assetType = KeyValueEntity<IOLInvestmentAssetTypeEnum, IOLInvestmentAssetType>.Default();
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

public class UpdateIOLInvestmentCommand : ICommand
{
    required public Guid Id { get; set; }
    required public string AssetSymbol { get; set; } = string.Empty;
    required public uint Alarms { get; set; } = 0;
    required public uint Quantity { get; set; } = 0;
    required public uint Assets { get; set; } = 0;
    required public decimal DailyVariation { get; set; } = 0M;
    required public decimal LastPrice { get; set; } = 0M;
    required public decimal AverageBuyPrice { get; set; } = 0M;
    required public decimal AverageReturnPercent { get; set; } = 0M;
    required public decimal AverageReturn { get; set; } = 0M;
    required public decimal Valued { get; set; } = 0M;
    required public IOLInvestmentAssetTypeEnum InvestmentAssetIOLTypeId { get; set; }
}
