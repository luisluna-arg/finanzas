using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.AppModules;

public class CreateAppModuleCommandHandler : BaseResponseHandler<CreateAppModuleCommand, AppModule>
{
    private readonly IRepository<AppModuleType, short> appModuleTypeRepository;
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;

    public CreateAppModuleCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IAppModuleRepository appModuleRepository,
        IRepository<AppModuleType, short> appModuleTypeRepository)
        : base(db)
    {
        this.appModuleTypeRepository = appModuleTypeRepository;
        this.appModuleRepository = appModuleRepository;
        this.currencyRepository = currencyRepository;
    }

    public override async Task<AppModule> Handle(CreateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var appModuleType = await appModuleTypeRepository.GetByIdAsync(command.AppModuleTypeId, cancellationToken);
        if (appModuleType == null) throw new Exception("App module type not found");

        var newAppModule = new AppModule()
        {
            CreatedAt = DateTime.UtcNow,
            Currency = currency,
            Name = command.Name,
            Type = appModuleType
        };

        await appModuleRepository.AddAsync(newAppModule, cancellationToken);

        return await Task.FromResult(newAppModule);
    }
}

public class CreateAppModuleCommand : IRequest<AppModule>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid CurrencyId { get; set; }
    [Required]
    public short AppModuleTypeId { get; set; }
}
