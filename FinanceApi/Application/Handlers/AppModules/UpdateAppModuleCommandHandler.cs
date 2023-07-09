using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.AppModules;

public class UpdateAppModuleCommandHandler : BaseResponseHandler<UpdateAppModuleCommand, AppModule>
{
    private readonly IRepository<AppModule, Guid> _appModuleRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository;

    public UpdateAppModuleCommandHandler(
        FinanceDbContext db,
        IRepository<AppModule, Guid> appModuleRepository,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        _appModuleRepository = appModuleRepository;
        _currencyRepository = currencyRepository;
    }

    public override async Task<AppModule> Handle(UpdateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var appModule = await _appModuleRepository.GetById(command.Id);
        var currency = await _currencyRepository.GetById(command.CurrencyId);

        appModule.Currency = currency;
        appModule.Name = command.Name;

        await DbContext.SaveChangesAsync();

        return await Task.FromResult(appModule);
    }
}
