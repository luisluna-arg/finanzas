using FinanceApi.Application.Commands.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class UpdateInvestmentAssetIOLCommandHandler : BaseResponseHandler<UpdateInvestmentAssetIOLCommand, InvestmentAssetIOL>
{
    private readonly IRepository<InvestmentAssetIOL, Guid> investmentAssetIOLRepository;
    private readonly IRepository<InvestmentAssetIOLType, ushort> investmentAssetIOLTypeRepository;

    public UpdateInvestmentAssetIOLCommandHandler(
        FinanceDbContext db,
        IRepository<InvestmentAssetIOL, Guid> investmentAssetIOLRepository,
        IRepository<InvestmentAssetIOLType, ushort> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.investmentAssetIOLRepository = investmentAssetIOLRepository;
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<InvestmentAssetIOL> Handle(UpdateInvestmentAssetIOLCommand command, CancellationToken cancellationToken)
    {
        var investmentAssetIOL = await investmentAssetIOLRepository.GetById(command.Id);

        var investmentAssetIOLType = await investmentAssetIOLTypeRepository.GetById((ushort)command.InvestmentAssetIOLTypeId);

        investmentAssetIOL.Asset = command.Asset;
        investmentAssetIOL.Alarms = command.Alarms;
        investmentAssetIOL.Quantity = command.Quantity;
        investmentAssetIOL.Assets = command.Assets;
        investmentAssetIOL.DailyVariation = command.DailyVariation;
        investmentAssetIOL.LastPrice = command.LastPrice;
        investmentAssetIOL.AverageBuyPrice = command.AverageBuyPrice;
        investmentAssetIOL.AverageReturnPercent = command.AverageReturnPercent;
        investmentAssetIOL.AverageReturn = command.AverageReturn;
        investmentAssetIOL.Valued = command.Valued;
        investmentAssetIOL.InvestmentAssetIOLType = investmentAssetIOLType;

        await investmentAssetIOLRepository.Update(investmentAssetIOL);

        return await Task.FromResult(investmentAssetIOL);
    }
}
