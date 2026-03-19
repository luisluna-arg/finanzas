using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.Debits.Base;
using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Repositories;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Commands.Debits;

public class CreateDebitCommand : UpsertDebitBaseCommand;

public class CreateDebitCommandHandler(
    FinanceDbContext db,
    IRepository<Debit, Guid> debitRepository,
    IRepository<DebitOrigin, Guid> debitOriginRepository,
    IDispatcher<FinanceDispatchContext> dispatcher
) : BaseCommandHandler<CreateDebitCommand, Debit>(db)
{
    private readonly IRepository<Debit, Guid> _debitRepository = debitRepository;
    private readonly IRepository<DebitOrigin, Guid> _debitOriginRepository = debitOriginRepository;
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher = dispatcher;

    public override async Task<DataResult<Debit>> ExecuteAsync(CreateDebitCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new CreateDebitCommandValidator());

        var originName = command.Origin.Trim();

        var origin = await _debitOriginRepository.GetByAsync(
            new Dictionary<string, object>
            {
                { "Name", originName },
                { "AppModuleId", command.AppModuleId }
            },
            cancellationToken);

        if (origin == null)
        {
            var createOriginCommand = new CreateDebitOriginCommand
            {
                Name = originName,
                AppModuleId = command.AppModuleId
            };

            var originResult = await _dispatcher.DispatchAsync<DataResult<DebitOrigin>>(createOriginCommand, null);
            origin = originResult.Data;
        }

        var debit = new Debit
        {
            Origin = origin!,
            Amount = command.Amount,
            TimeStamp = DateTime.UtcNow,
            Frequency = command.Frequency,
            Deactivated = command.Deactivated,
        };

        await _debitRepository.AddAsync(debit, cancellationToken);

        return DataResult<Debit>.Success(debit);
    }
}

public class CreateDebitCommandValidator : UpsertDebitBaseCommandValidator<CreateDebitCommand>;
