using FinanceApi.Application.Commands.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class CreateInvestmentAssetIOLCommandHandler : BaseResponseHandler<CreateInvestmentAssetIOLCommand, InvestmentAssetIOL>
{
    private readonly IRepository<InvestmentAssetIOL, Guid> investmentAssetIOLRepository;
    private readonly IRepository<InvestmentAssetIOLType, ushort> investmentAssetIOLTypeRepository;

    public CreateInvestmentAssetIOLCommandHandler(
        FinanceDbContext db,
        IRepository<InvestmentAssetIOL, Guid> investmentAssetIOLRepository,
        IRepository<InvestmentAssetIOLType, ushort> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.investmentAssetIOLRepository = investmentAssetIOLRepository;
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<InvestmentAssetIOL> Handle(CreateInvestmentAssetIOLCommand command, CancellationToken cancellationToken)
    {
        var investmentAssetIOLType = await investmentAssetIOLTypeRepository.GetById((ushort)command.InvestmentAssetIOLTypeId);

        var newInvestmentAssetIOL = new InvestmentAssetIOL()
        {
            Asset = command.Asset,
            Alarms = command.Alarms,
            Quantity = command.Quantity,
            Assets = command.Assets,
            DailyVariation = command.DailyVariation,
            LastPrice = command.LastPrice,
            AverageBuyPrice = command.AverageBuyPrice,
            AverageReturnPercent = command.AverageReturnPercent,
            AverageReturn = command.AverageReturn,
            Valued = command.Valued,
            InvestmentAssetIOLType = investmentAssetIOLType
        };

        await investmentAssetIOLRepository.Add(newInvestmentAssetIOL);

        return await Task.FromResult(newInvestmentAssetIOL);
    }
}
