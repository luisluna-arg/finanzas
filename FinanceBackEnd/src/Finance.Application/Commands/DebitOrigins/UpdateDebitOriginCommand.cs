using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.DebitOrigins.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Debits;
using Finance.Persistence;
using FluentValidation;

namespace Finance.Application.Commands.DebitOrigins;

public class UpdateDebitOriginCommand : UpsertDebitOriginBaseCommand
{
    [Required]
    public Guid Id { get; set; }
}

public class UpdateDebitOriginCommandHandler(
    FinanceDbContext db,
    IRepository<AppModule, Guid> appModuleRepository,
    IRepository<DebitOrigin, Guid> debitOriginRepository
) : BaseCommandHandler<UpdateDebitOriginCommand, DebitOrigin>(db)
{
    public override async Task<DataResult<DebitOrigin>> ExecuteAsync(UpdateDebitOriginCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new UpdateDebitOriginCommandValidator());

        var debitOrigin = await debitOriginRepository.GetByIdAsync(command.Id, cancellationToken);
        if (debitOrigin == null) throw new Exception("Debit Origin not found");

        var appModule = await appModuleRepository.GetByIdAsync(command.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception("App module not found");

        debitOrigin.AppModule = appModule;
        debitOrigin.Name = command.Name.Trim();
        debitOrigin.Deactivated = command.Deactivated;

        await debitOriginRepository.UpdateAsync(debitOrigin, cancellationToken);

        return DataResult<DebitOrigin>.Success(debitOrigin);
    }
}

public class UpdateDebitOriginCommandValidator : UpsertDebitOriginBaseCommandValidator<UpdateDebitOriginCommand>
{
    public UpdateDebitOriginCommandValidator() : base()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Debit Origin Id is required.");
    }
}
