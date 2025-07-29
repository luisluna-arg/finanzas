using Finance.Application.Base.Handlers;
using CQRSDispatch;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Commands.Movements;

public class CreateMovementCommandHandler : BaseCommandHandler<CreateMovementCommand, Movement>
{
    private readonly IRepository<Movement, Guid> _movementRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IAppModuleRepository _appModuleRepository;

    public CreateMovementCommandHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository,
        IRepository<Currency, Guid> currencyRepository,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        _movementRepository = movementRepository;
        _currencyRepository = currencyRepository;
        _appModuleRepository = appModuleRepository;
    }

    public override async Task<DataResult<Movement>> ExecuteAsync(CreateMovementCommand command, CancellationToken cancellationToken)
    {
        AppModule? appModule = command.AppModuleId.HasValue ? await _appModuleRepository.GetByIdAsync(command.AppModuleId.Value, cancellationToken) : await _appModuleRepository.GetFundsAsync(cancellationToken);
        if (appModule == null) throw new Exception("AppModule not found");

        Currency? currency = command.CurrencyId.HasValue ? await _currencyRepository.GetByIdAsync(command.CurrencyId.Value, cancellationToken) : null;
        if (currency == null) throw new Exception("Currency not found");

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

        await _movementRepository.AddAsync(newMovement, cancellationToken);

        return DataResult<Movement>.Success(newMovement);
    }
}

public class CreateMovementCommand : CreateMovementBaseCommand
{
    public Guid? AppModuleId { get; set; }
}
