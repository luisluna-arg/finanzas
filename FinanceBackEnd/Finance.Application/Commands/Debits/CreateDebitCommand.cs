using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Application.Commands.DebitOrigins;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Finance.Domain.Enums;
using Finance.Application.Auth;

namespace Finance.Application.Commands.Debits;

public class CreateDebitCommandHandler : BaseCommandHandler<CreateDebitCommand, Debit>
{
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher;
    private readonly IRepository<Debit, Guid> _debitRepository;
    private readonly IRepository<DebitOrigin, Guid> _debitOriginRepository;

    public CreateDebitCommandHandler(
        FinanceDbContext db,
        IDispatcher<FinanceDispatchContext> dispatcher,
        IRepository<Debit, Guid> debitRepository,
        IRepository<DebitOrigin, Guid> debitOriginRepository)
        : base(db)
    {
        _dispatcher = dispatcher;
        _debitRepository = debitRepository;
        _debitOriginRepository = debitOriginRepository;
    }

    public override async Task<DataResult<Debit>> ExecuteAsync(CreateDebitCommand command, CancellationToken cancellationToken)
    {
        var originName = command.Origin.Trim();

        var origin = await _debitOriginRepository.GetByAsync(
            new Dictionary<string, object>()
            {
                { "Name", originName },
                { "AppModuleId", command.AppModuleId }
            },
            cancellationToken);

        if (origin == null)
        {
            var createDebitOriginCommand = new CreateDebitOriginCommand()
            {
                Name = originName,
                AppModuleId = command.AppModuleId
            };

            DataResult<DebitOrigin> result = await _dispatcher.DispatchAsync(createDebitOriginCommand);
            origin = result.Data;
        }

        var newDebit = new Debit()
        {
            Origin = origin!,
            Amount = command.Amount,
            TimeStamp = DateTime.UtcNow,
            Frequency = command.Frequency
        };

        await _debitRepository.AddAsync(newDebit, cancellationToken);

        return DataResult<Debit>.Success(newDebit);
    }
}

public class CreateDebitCommand : ICommand
{
    public Guid AppModuleId { get; set; }

    [Required]
    public string Origin { get; set; } = string.Empty;

    [Required]
    public decimal Amount { get; set; } = 0m;

    [Required]
    public bool Deactivated { get; set; }

    [Required]
    public FrequencyEnum Frequency { get; set; }
}
