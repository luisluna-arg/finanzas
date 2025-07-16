using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Finance.Persistance.Constants;

namespace Finance.Application.Commands.IOLInvestments;

public class CreateIOLInvestmentCommandHandler : BaseCommandHandler<CreateIOLInvestmentCommand, IOLInvestment>
{
    private readonly IRepository<IOLInvestmentAsset, Guid> iolInvestmentAssetRepository;
    private readonly IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository;
    private readonly IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> investmentAssetIOLTypeRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;

    public CreateIOLInvestmentCommandHandler(
        FinanceDbContext db,
        IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLRepository,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository,
        IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> investmentAssetIOLTypeRepository,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.iolInvestmentAssetRepository = investmentAssetIOLRepository;
        this.investmentAssetIOLRecordRepository = investmentAssetIOLRecordRepository;
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
        this.currencyRepository = currencyRepository;
    }

    public override async Task<DataResult<IOLInvestment>> ExecuteAsync(CreateIOLInvestmentCommand command, CancellationToken cancellationToken)
    {
        var newInvestmentAssetIOL = new IOLInvestment()
        {
            Asset = await GetAssetAsync(command, cancellationToken),
            CreatedAt = DateTime.UtcNow,
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

        await investmentAssetIOLRecordRepository.AddAsync(newInvestmentAssetIOL, cancellationToken);

        return DataResult<IOLInvestment>.Success(newInvestmentAssetIOL);
    }

    private async Task<IOLInvestmentAsset> GetAssetAsync(CreateIOLInvestmentCommand command, CancellationToken cancellationToken)
    {
        var asset = await iolInvestmentAssetRepository.GetByAsync("Symbol", command.AssetSymbol, cancellationToken);

        if (asset == null)
        {
            var assetType = await investmentAssetIOLTypeRepository.GetByAsync("Name", IOLInvestmentAssetType.DefaultName, cancellationToken);

            if (assetType == null)
            {
                assetType = KeyValueEntity<IOLInvestmentAssetTypeEnum>.Default<IOLInvestmentAssetType>();
            }

            var currency = (await currencyRepository.GetByIdAsync(command.CurrencyId ?? Guid.Parse(CurrencyConstants.PesoId), cancellationToken))!;

            asset = new IOLInvestmentAsset()
            {
                Symbol = command.AssetSymbol,
                Description = command.AssetSymbol,
                Currency = currency,
                Type = assetType
            };
        }

        return asset!;
    }
}

public class CreateIOLInvestmentCommand : ICommand
{
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

    public Guid? CurrencyId { get; set; }
}
