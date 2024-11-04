using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.DebitOrigins;

public class CreateDebitOriginCommandHandler : BaseResponseHandler<CreateDebitOriginCommand, DebitOrigin>
{
    private readonly IAppModuleRepository appModuleRepository;

    private readonly IRepository<DebitOrigin, Guid> debitOriginRepository;

    public CreateDebitOriginCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<DebitOrigin, Guid> debitOriginRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
        this.debitOriginRepository = debitOriginRepository;
    }

    public override async Task<DebitOrigin> Handle(CreateDebitOriginCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetByIdAsync(command.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception("App module not found");

        var newDebitOrigin = new DebitOrigin()
        {
            AppModule = appModule,
            Name = command.Name,
            Deactivated = command.Deactivated
        };

        await debitOriginRepository.AddAsync(newDebitOrigin, cancellationToken);

        return await Task.FromResult(newDebitOrigin);
    }
}

public class CreateDebitOriginCommand : IRequest<DebitOrigin>
{
    [Required]
    public Guid AppModuleId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool Deactivated { get; set; }
}
