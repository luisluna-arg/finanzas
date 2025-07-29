using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Domain.Enums;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Commands.AppModules;

public class CreateAppModuleCommandHandler : BaseCommandHandler<CreateAppModuleCommand, AppModule>
{
    private readonly IRepository<AppModuleType, AppModuleTypeEnum> appModuleTypeRepository;
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;

    public CreateAppModuleCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IAppModuleRepository appModuleRepository,
        IRepository<AppModuleType, AppModuleTypeEnum> appModuleTypeRepository)
        : base(db)
    {
        this.appModuleTypeRepository = appModuleTypeRepository;
        this.appModuleRepository = appModuleRepository;
        this.currencyRepository = currencyRepository;
    }

    public override async Task<DataResult<AppModule>> ExecuteAsync(CreateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var appModuleType = await appModuleTypeRepository.GetByIdAsync(command.AppModuleTypeId, cancellationToken);
        if (appModuleType == null) throw new Exception("App module type not found");

        var newAppModule = new AppModule()
        {
            Currency = currency,
            Name = command.Name,
            Type = appModuleType
        };

        await appModuleRepository.AddAsync(newAppModule, cancellationToken);

        return new DataResult<AppModule>(true, newAppModule, "App module created successfully");
    }
}

public class CreateAppModuleCommand : ICommand
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid CurrencyId { get; set; }
    [Required]
    public AppModuleTypeEnum AppModuleTypeId { get; set; }
}
