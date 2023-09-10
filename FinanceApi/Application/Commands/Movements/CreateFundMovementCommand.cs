using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Commands.Movements;

public class CreateFundMovementCommandHandler : BaseResponseHandler<CreateFundMovementCommand, Movement>
{
    private readonly IRepository<Movement, Guid> movementRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IAppModuleRepository appModuleRepository;

    public CreateFundMovementCommandHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository,
        IRepository<Currency, Guid> currencyRepository,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
        this.currencyRepository = currencyRepository;
        this.appModuleRepository = appModuleRepository;
    }

    public override async Task<Movement> Handle(CreateFundMovementCommand command, CancellationToken cancellationToken)
    {
        AppModule? appModule = await this.appModuleRepository.GetFund();

        Currency? currency = command.CurrencyId.HasValue ? await this.currencyRepository.GetById(command.CurrencyId.Value) : null;

        var newMovement = new Movement()
        {
            AppModule = appModule,
            Currency = currency,
            Amount = command.Amount,
            Concept1 = command.Concept1,
            Concept2 = command.Concept2,
            TimeStamp = command.TimeStamp,
            Total = command.Total,
        };

        await movementRepository.Add(newMovement);

        return await Task.FromResult(newMovement);
    }
}

public class CreateFundMovementCommand : CreateMovementBaseCommand
{
}
