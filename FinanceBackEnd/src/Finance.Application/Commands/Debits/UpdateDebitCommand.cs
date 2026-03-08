using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.Debits.Base;
using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Repositories;
using Finance.Domain.Models.Debits;
using Finance.Persistence;
using FluentValidation;

namespace Finance.Application.Commands.Debits;

public class UpdateDebitCommand : UpsertDebitBaseCommand
{
    [Required]
    public Guid Id { get; set; }
}

public class UpdateDebitCommandHandler(
    FinanceDbContext db,
    IRepository<Debit, Guid> debitRepository,
    IRepository<DebitOrigin, Guid> debitOriginRepository,
    IDispatcher<FinanceDispatchContext> dispatcher
) : BaseCommandHandler<UpdateDebitCommand, Debit>(db)
{
    private readonly IRepository<Debit, Guid> _debitRepository = debitRepository;
    private readonly IRepository<DebitOrigin, Guid> _debitOriginRepository = debitOriginRepository;
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher = dispatcher;

    public override async Task<DataResult<Debit>> ExecuteAsync(UpdateDebitCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new UpdateDebitCommandValidator());

        var debit = await _debitRepository.GetByIdAsync(command.Id, cancellationToken);
        if (debit == null) throw new Exception("Debit not found");

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

            var originResult = await _dispatcher.DispatchAsync<DataResult<DebitOrigin>>(createOriginCommand);
            origin = originResult.Data;
        }

        debit.Origin = origin!;
        debit.Amount = command.Amount;
        debit.Frequency = command.Frequency;
        debit.Deactivated = command.Deactivated;

        await _debitRepository.UpdateAsync(debit, cancellationToken);

        return DataResult<Debit>.Success(debit);
    }
}

public class UpdateDebitCommandValidator : UpsertDebitBaseCommandValidator<UpdateDebitCommand>
{
    public UpdateDebitCommandValidator() : base()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Debit Id is required.");
    }
}
