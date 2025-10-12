using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Commands.DebitOrigins;

public class CreateDebitOriginCommandHandler : BaseCommandHandler<CreateDebitOriginCommand, DebitOrigin>
{
    private readonly IAppModuleRepository _appModuleRepository;
    private readonly IRepository<DebitOrigin, Guid> _debitOriginRepository;

    public CreateDebitOriginCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<DebitOrigin, Guid> debitOriginRepository)
        : base(db)
    {
        _appModuleRepository = appModuleRepository;
        _debitOriginRepository = debitOriginRepository;
    }

    public override async Task<DataResult<DebitOrigin>> ExecuteAsync(CreateDebitOriginCommand command, CancellationToken cancellationToken)
    {
        var appModule = await _appModuleRepository.GetByIdAsync(command.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception("App module not found");

        var newDebitOrigin = new DebitOrigin()
        {
            AppModule = appModule,
            Name = command.Name,
            Deactivated = command.Deactivated
        };

        await _debitOriginRepository.AddAsync(newDebitOrigin, cancellationToken);

        return DataResult<DebitOrigin>.Success(newDebitOrigin);
    }
}

public class CreateDebitOriginCommand : ICommand<DataResult<DebitOrigin>>
{
    [Required]
    public Guid AppModuleId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool Deactivated { get; set; }
}
