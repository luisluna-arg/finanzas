using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;
using MediatR;

namespace FinanceApi.Application.Commands.AppModules;

public class UpdateAppModuleCommandHandler : BaseResponseHandler<UpdateAppModuleCommand, AppModule>
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

    public override async Task<AppModule> Handle(UpdateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetById(command.Id);
        var currency = await currencyRepository.GetById(command.CurrencyId);

        appModule.Currency = currency;
        appModule.Name = command.Name;

        await appModuleRepository.Update(appModule);

        return await Task.FromResult(appModule);
    }
}

public class UpdateAppModuleCommand : IRequest<AppModule>
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid CurrencyId { get; set; }
}
