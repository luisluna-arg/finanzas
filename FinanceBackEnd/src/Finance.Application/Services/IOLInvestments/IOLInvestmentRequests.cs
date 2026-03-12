using Finance.Domain.Enums;

namespace Finance.Application.Services.IOLInvestments;

public sealed record CreateIOLInvestmentRequest(
    string AssetSymbol,
    uint Alarms,
    uint Quantity,
    uint Assets,
    decimal DailyVariation,
    decimal LastPrice,
    decimal AverageBuyPrice,
    decimal AverageReturnPercent,
    decimal AverageReturn,
    decimal Valued,
    IOLInvestmentAssetTypeEnum InvestmentAssetIOLTypeId,
    Guid? CurrencyId = null);

public sealed record UpdateIOLInvestmentRequest(
    Guid Id,
    string AssetSymbol,
    uint Alarms,
    uint Quantity,
    uint Assets,
    decimal DailyVariation,
    decimal LastPrice,
    decimal AverageBuyPrice,
    decimal AverageReturnPercent,
    decimal AverageReturn,
    decimal Valued,
    IOLInvestmentAssetTypeEnum InvestmentAssetIOLTypeId);

public sealed record DeleteIOLInvestmentRequest(Guid[] Ids);
