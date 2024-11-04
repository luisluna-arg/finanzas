using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Commands.Movements;

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
        AppModule? appModule = command.AppModuleId.HasValue ? await appModuleRepository.GetByIdAsync(command.AppModuleId.Value, cancellationToken) : await appModuleRepository.GetFundsAsync(cancellationToken);
        if (appModule == null) throw new Exception("AppModule not found");

        Currency? currency = command.CurrencyId.HasValue ? await currencyRepository.GetByIdAsync(command.CurrencyId.Value, cancellationToken) : null;
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

        await this.movementRepository.AddAsync(newMovement, cancellationToken);

        return await Task.FromResult(newMovement);
    }
}

public class CreateMovementCommand : CreateMovementBaseCommand
{
    public Guid? AppModuleId { get; set; }
}
