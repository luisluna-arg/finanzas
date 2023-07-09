using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.AppModules;

public class CreateAppModuleCommandHandler : BaseResponseHandler<CreateAppModuleCommand, AppModule>
{
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;

    public CreateAppModuleCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
        this.appModuleRepository = appModuleRepository;
    }

    public override async Task<AppModule> Handle(CreateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var currency = await currencyRepository.GetById(command.CurrencyId);

        var newAppModule = new AppModule()
        {
            CreatedAt = DateTime.UtcNow,
            Currency = currency,
            Name = command.Name
        };

        await appModuleRepository.Add(newAppModule);

        return await Task.FromResult(newAppModule);
    }
}
