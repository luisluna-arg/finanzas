using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Commands.IOLInvestments;

public class CreateIOLInvestmentAssetCommand : ICommand<DataResult<IOLInvestmentAsset>>
{
    public required IOLInvestmentAssetTypeEnum TypeId { get; set; }
    public required Guid CurrencyId { get; set; }
    public required string Symbol { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
}

public class CreateIOLInvestmentAssetCommandHandler(
    FinanceDbContext db,
    IRepository<IOLInvestmentAsset, Guid> repository,
    IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> assetTypeRepository,
    IRepository<Currency, Guid> currencyRepository)
    : BaseCommandHandler<CreateIOLInvestmentAssetCommand, IOLInvestmentAsset>(db)
{
    public override async Task<DataResult<IOLInvestmentAsset>> ExecuteAsync(
        CreateIOLInvestmentAssetCommand command, CancellationToken cancellationToken)
    {
        var assetType = await assetTypeRepository.GetByIdAsync(command.TypeId, cancellationToken)
            ?? throw new Exception("IOL Investment Asset Type not found");

        var currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken)
            ?? throw new Exception("Currency not found");

        var asset = new IOLInvestmentAsset
        {
            TypeId = command.TypeId,
            CurrencyId = command.CurrencyId,
            Symbol = command.Symbol,
            Description = command.Description,
            Type = assetType,
            Currency = currency,
        };

        await repository.AddAsync(asset, cancellationToken);

        return DataResult<IOLInvestmentAsset>.Success(asset);
    }
}
