using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Commands.Movements;

public class CreateMovementCommandHandler : BaseResponseHandler<CreateMovementCommand, Movement>
{
    private readonly IRepository<Movement, Guid> movementRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IAppModuleRepository appModuleRepository;

    public CreateMovementCommandHandler(
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

    public override async Task<Movement> Handle(CreateMovementCommand command, CancellationToken cancellationToken)
    {
        AppModule? appModule = command.AppModuleId.HasValue ? await appModuleRepository.GetById(command.AppModuleId.Value) : await appModuleRepository.GetFunds();
        if (appModule == null) throw new Exception("AppModule not found");

        Currency? currency = command.CurrencyId.HasValue ? await currencyRepository.GetById(command.CurrencyId.Value) : null;
        if (currency == null) throw new Exception("Currency not found");

        var newMovement = new Movement()
        {
            AppModule = appModule,
            Currency = currency,
            Amount = command.Amount,
            Concept1 = command.Concept1,
            Concept2 = command.Concept2,
            TimeStamp = command.TimeStamp,
            CreatedAt = DateTime.UtcNow,
            Total = command.Total,
        };

        await this.movementRepository.Add(newMovement);

        return await Task.FromResult(newMovement);
    }
}

public class CreateMovementCommand : CreateMovementBaseCommand
{
    public Guid? AppModuleId { get; set; }
}
