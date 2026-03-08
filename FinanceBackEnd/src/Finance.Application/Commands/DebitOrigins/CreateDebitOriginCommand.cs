using CQRSDispatch;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.DebitOrigins.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Commands.DebitOrigins;

public class CreateDebitOriginCommand : UpsertDebitOriginBaseCommand;

public class CreateDebitOriginCommandHandler(
    FinanceDbContext db,
    IRepository<AppModule, Guid> appModuleRepository,
    IRepository<DebitOrigin, Guid> debitOriginRepository
) : BaseCommandHandler<CreateDebitOriginCommand, DebitOrigin>(db)
{
    public override async Task<DataResult<DebitOrigin>> ExecuteAsync(CreateDebitOriginCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new CreateDebitOriginCommandValidator());

        var appModule = await appModuleRepository.GetByIdAsync(command.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception("App module not found");

        var debitOrigin = new DebitOrigin
        {
            AppModule = appModule,
            Name = command.Name.Trim(),
            Deactivated = command.Deactivated,
        };

        await debitOriginRepository.AddAsync(debitOrigin, cancellationToken);

        return DataResult<DebitOrigin>.Success(debitOrigin);
    }
}

public class CreateDebitOriginCommandValidator : UpsertDebitOriginBaseCommandValidator<CreateDebitOriginCommand>;
