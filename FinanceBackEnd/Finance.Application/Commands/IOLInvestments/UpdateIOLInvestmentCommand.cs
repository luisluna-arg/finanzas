using Finance.Application.Base.Handlers;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.IOLInvestments;

public class UpdateIOLInvestmentCommandHandler : BaseResponseHandler<UpdateIOLInvestmentCommand, IOLInvestment>
{
    private readonly IRepository<IOLInvestmentAsset, Guid> iolInvestmentAssetRepository;
    private readonly IRepository<IOLInvestment, Guid> iolInvestmentRepository;
    private readonly IRepository<IOLInvestmentAssetType, ushort> iolInvestmentAssetTypeRepository;

    public UpdateIOLInvestmentCommandHandler(
        FinanceDbContext db,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository,
        IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLRepository,
        IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.iolInvestmentAssetRepository = investmentAssetIOLRepository;
        this.iolInvestmentRepository = investmentAssetIOLRecordRepository;
        this.iolInvestmentAssetTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<IOLInvestment> Handle(UpdateIOLInvestmentCommand command, CancellationToken cancellationToken)
    {
        var iolInvestmentAsset = await iolInvestmentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (iolInvestmentAsset == null) throw new Exception("IOL Investment Asset not found");

        iolInvestmentAsset.Asset = await GetAssetAsync(command.AssetSymbol, cancellationToken);
        iolInvestmentAsset.Alarms = command.Alarms;
        iolInvestmentAsset.Quantity = command.Quantity;
        iolInvestmentAsset.Assets = command.Assets;
        iolInvestmentAsset.DailyVariation = command.DailyVariation;
        iolInvestmentAsset.LastPrice = command.LastPrice;
        iolInvestmentAsset.AverageBuyPrice = command.AverageBuyPrice;
        iolInvestmentAsset.AverageReturnPercent = command.AverageReturnPercent;
        iolInvestmentAsset.AverageReturn = command.AverageReturn;
        iolInvestmentAsset.Valued = command.Valued;

        await iolInvestmentRepository.UpdateAsync(iolInvestmentAsset, cancellationToken);

        return await Task.FromResult(iolInvestmentAsset);
    }

    private async Task<IOLInvestmentAsset> GetAssetAsync(string assetSymbol, CancellationToken cancellationToken)
    {
        var asset = await iolInvestmentAssetRepository.GetByAsync("Symbol", assetSymbol, cancellationToken);

        if (asset == null)
        {
            var assetType = await iolInvestmentAssetTypeRepository.GetByAsync("Name", IOLInvestmentAssetType.Default, cancellationToken);

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

public class UpdateIOLInvestmentCommand : IRequest<IOLInvestment>
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
