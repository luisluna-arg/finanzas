using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Commands.DebitOrigins;

public class UpdateDebitOriginCommandHandler : BaseCommandHandler<UpdateDebitOriginCommand, DebitOrigin>
{
    private readonly IAppModuleRepository _appModuleRepository;
    private readonly IRepository<DebitOrigin, Guid> _debitOriginRepository;

    public UpdateDebitOriginCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<DebitOrigin, Guid> debitOriginRepository)
        : base(db)
    {
        _appModuleRepository = appModuleRepository;
        _debitOriginRepository = debitOriginRepository;
    }

    public override async Task<DataResult<DebitOrigin>> ExecuteAsync(UpdateDebitOriginCommand command, CancellationToken cancellationToken)
    {
        var debitOrigin = await _debitOriginRepository.GetByIdAsync(command.Id, cancellationToken);
        if (debitOrigin == null) throw new Exception("Debit Origin not found");

        var appModule = await _appModuleRepository.GetByIdAsync(command.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception("App module not found");

        debitOrigin.AppModule = appModule;
        debitOrigin.Name = command.Name;
        debitOrigin.Deactivated = command.Deactivated;

        await _debitOriginRepository.UpdateAsync(debitOrigin, cancellationToken);

        return DataResult<DebitOrigin>.Success(debitOrigin);
    }
}

public class UpdateDebitOriginCommand : ICommand
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
