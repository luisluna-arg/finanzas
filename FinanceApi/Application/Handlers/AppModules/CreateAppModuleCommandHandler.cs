using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.AppModules;

public class CreateAppModuleCommandHandler : BaseResponseHandler<CreateAppModuleCommand, AppModule>
{
    private readonly IRepository<Currency, Guid> _currencyRepository;

    public CreateAppModuleCommandHandler(FinanceDbContext db, IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        _currencyRepository = currencyRepository;
    }

    public override async Task<AppModule> Handle(CreateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.GetById(command.CurrencyId);

        var newAppModule = new AppModule()
        {
            CreatedAt = DateTime.UtcNow,
            Currency = currency,
            Name = command.Name
        };

        DbContext.AppModule.Add(newAppModule);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(newAppModule);
    }
}
