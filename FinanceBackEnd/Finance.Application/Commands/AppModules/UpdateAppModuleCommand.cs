using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Persistence;

namespace Finance.Application.Commands.AppModules;

public class UpdateAppModuleCommandHandler : BaseCommandHandler<UpdateAppModuleCommand, AppModule>
{
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;

    public UpdateAppModuleCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
        this.currencyRepository = currencyRepository;
    }

    public override async Task<DataResult<AppModule>> ExecuteAsync(UpdateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (appModule == null) throw new Exception("App Module not found");

        var currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        appModule.Currency = currency;
        appModule.Name = command.Name;

        await appModuleRepository.UpdateAsync(appModule, cancellationToken);

        return DataResult<AppModule>.Success(appModule);
    }
}

public class UpdateAppModuleCommand : ICommand
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid CurrencyId { get; set; }
}
