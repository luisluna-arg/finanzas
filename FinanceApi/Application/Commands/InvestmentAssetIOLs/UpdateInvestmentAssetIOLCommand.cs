using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.InvestmentAssetIOLs;

public class UpdateInvestmentAssetIOLCommand : IRequest<InvestmentAssetIOL>
{
    required public Guid Id { get; set; }

    required public string Asset { get; set; } = string.Empty;

    required public uint Alarms { get; set; } = 0;

    required public uint Quantity { get; set; } = 0;

    required public uint Assets { get; set; } = 0;

    required public decimal DailyVariation { get; set; } = 0M;

    required public decimal LastPrice { get; set; } = 0M;

    required public decimal AverageBuyPrice { get; set; } = 0M;

    required public decimal AverageReturnPercent { get; set; } = 0M;

    required public decimal AverageReturn { get; set; } = 0M;

    required public decimal Valued { get; set; } = 0M;

    required public InvestmentAssetIOLTypeEnum InvestmentAssetIOLTypeId { get; set; }
}
