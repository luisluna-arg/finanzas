using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.DebitOrigins;

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
        var debitOrigin = await debitOriginRepository.GetByIdAsync(command.Id, cancellationToken);
        if (debitOrigin == null) throw new Exception("Debit Origin not found");

        var appModule = await appModuleRepository.GetByIdAsync(command.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception("App module not found");

        debitOrigin.AppModule = appModule;
        debitOrigin.Name = command.Name;
        debitOrigin.Deactivated = command.Deactivated;

        await debitOriginRepository.UpdateAsync(debitOrigin, cancellationToken);

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
