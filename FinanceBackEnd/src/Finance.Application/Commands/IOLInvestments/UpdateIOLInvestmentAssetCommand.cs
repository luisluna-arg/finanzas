using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public class UpdateIOLInvestmentAssetCommandHandler(
    FinanceDbContext db,
    IRepository<IOLInvestmentAsset, Guid> repository,
    IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> assetTypeRepository,
    IRepository<Currency, Guid> currencyRepository)
    : BaseCommandHandler<UpdateIOLInvestmentAssetCommand, IOLInvestmentAsset>(db)
{
    public override async Task<DataResult<IOLInvestmentAsset>> ExecuteAsync(
        UpdateIOLInvestmentAssetCommand command, CancellationToken cancellationToken)
    {
        var asset = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new Exception("IOL Investment Asset not found");

        var assetType = await assetTypeRepository.GetByIdAsync(command.TypeId, cancellationToken)
            ?? throw new Exception("IOL Investment Asset Type not found");

        var currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken)
            ?? throw new Exception("Currency not found");

        asset.TypeId = command.TypeId;
        asset.CurrencyId = command.CurrencyId;
        asset.Symbol = command.Symbol;
        asset.Description = command.Description;
        asset.Type = assetType;
        asset.Currency = currency;

        await repository.UpdateAsync(asset, cancellationToken);

        return DataResult<IOLInvestmentAsset>.Success(asset);
    }
}

public class UpdateIOLInvestmentAssetCommand : ICommand<DataResult<IOLInvestmentAsset>>
{
    public required Guid Id { get; set; }
    public required IOLInvestmentAssetTypeEnum TypeId { get; set; }
    public required Guid CurrencyId { get; set; }
    public required string Symbol { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
}
