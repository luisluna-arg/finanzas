using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.DebitOrigins;

public class UpdateDebitOriginCommandHandler : BaseResponseHandler<UpdateDebitOriginCommand, DebitOrigin>
{
    private readonly IAppModuleRepository appModuleRepository;

    private readonly IRepository<DebitOrigin, Guid> debitOriginRepository;

    public UpdateDebitOriginCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<DebitOrigin, Guid> debitOriginRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
        this.debitOriginRepository = debitOriginRepository;
    }

    public override async Task<DebitOrigin> Handle(UpdateDebitOriginCommand command, CancellationToken cancellationToken)
    {
        var debitOrigin = await debitOriginRepository.GetById(command.Id);
        if (debitOrigin == null) throw new Exception("Debit Origin not found");

        var appModule = await appModuleRepository.GetById(command.AppModuleId);
        if (appModule == null) throw new Exception("App module not found");

        debitOrigin.AppModule = appModule;
        debitOrigin.Name = command.Name;
        debitOrigin.Deactivated = command.Deactivated;

        await debitOriginRepository.Update(debitOrigin);

        return await Task.FromResult(debitOrigin);
    }
}

public class UpdateDebitOriginCommand : IRequest<DebitOrigin>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid AppModuleId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool Deactivated { get; set; }
}
