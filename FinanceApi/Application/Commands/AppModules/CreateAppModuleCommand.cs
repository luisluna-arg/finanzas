using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.AppModules;

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
        if (currency == null) throw new Exception("Currency not found");

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

public class CreateAppModuleCommand : IRequest<AppModule>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid CurrencyId { get; set; }
}
