using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;
using MediatR;

namespace FinanceApi.Application.Commands.IOLInvestments;

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
        var investmentAssetIOL = await iolInvestmentRepository.GetById(command.Id);

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

        await iolInvestmentRepository.Update(investmentAssetIOL);

        return await Task.FromResult(investmentAssetIOL);
    }

    private async Task<IOLInvestmentAsset> GetAsset(string assetSymbol)
    {
        var asset = await iolInvestmentAssetRepository.GetBy("Symbol", assetSymbol);

        if (asset == null)
        {
            var assetType = await iolInvestmentAssetTypeRepository.GetBy("Name", IOLInvestmentAssetType.Default);

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
