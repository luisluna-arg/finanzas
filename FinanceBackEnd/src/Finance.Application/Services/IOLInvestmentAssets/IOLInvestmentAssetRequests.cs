using Finance.Domain.Enums;

namespace Finance.Application.Services.IOLInvestmentAssets;

public sealed record CreateIOLInvestmentAssetRequest(
    IOLInvestmentAssetTypeEnum TypeId,
    Guid CurrencyId,
    string Symbol,
    string Description);

public sealed record UpdateIOLInvestmentAssetRequest(
    Guid Id,
    IOLInvestmentAssetTypeEnum TypeId,
    Guid CurrencyId,
    string Symbol,
    string Description);

public sealed record DeleteIOLInvestmentAssetRequest(Guid[] Ids);
