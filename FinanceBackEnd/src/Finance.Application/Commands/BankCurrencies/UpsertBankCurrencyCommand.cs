using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.BankCurrencies;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.BankCurrencies;

public class UpsertBankCurrencyCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<BankCurrency>>
{
    public Guid BankId { get; set; }
    public Guid CurrencyId { get; set; }
    public bool DailyUse { get; set; }
    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public class UpsertBankCurrencyCommandValidator : AbstractValidator<UpsertBankCurrencyCommand>
{
    public UpsertBankCurrencyCommandValidator()
    {
        RuleFor(x => x.BankId).NotEmpty().WithMessage("Bank Id is required.");
        RuleFor(x => x.CurrencyId).NotEmpty().WithMessage("Currency Id is required.");
    }
}

public class UpsertBankCurrencyCommandHandler(FinanceDbContext db) : BaseCommandHandler<UpsertBankCurrencyCommand, BankCurrency>(db)
{
    public override async Task<DataResult<BankCurrency>> ExecuteAsync(UpsertBankCurrencyCommand command, CancellationToken cancellationToken = default)
    {
        command.ThrowIfNotValid(new UpsertBankCurrencyCommandValidator());

        var existing = await DbContext.BankCurrency
            .FirstOrDefaultAsync(o => o.BankId == command.BankId && o.CurrencyId == command.CurrencyId, cancellationToken);

        if (existing != null)
        {
            existing.DailyUse = command.DailyUse;
            await DbContext.SaveChangesAsync(cancellationToken);
            return DataResult<BankCurrency>.Success(existing);
        }

        var bank = await DbContext.Bank.FirstOrDefaultAsync(o => o.Id == command.BankId, cancellationToken);
        if (bank == null) return DataResult<BankCurrency>.Failure("Bank not found");

        var currency = await DbContext.Currency.FirstOrDefaultAsync(o => o.Id == command.CurrencyId, cancellationToken);
        if (currency == null) return DataResult<BankCurrency>.Failure("Currency not found");

        var bankCurrency = new BankCurrency
        {
            BankId = command.BankId,
            CurrencyId = command.CurrencyId,
            DailyUse = command.DailyUse,
        };

        DbContext.BankCurrency.Add(bankCurrency);
        DbContext.BankCurrencyPermissions.Add(new BankCurrencyPermissions
        {
            BankId = command.BankId,
            CurrencyId = command.CurrencyId,
            UserId = command.Context.UserInfo.Id,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });

        await DbContext.SaveChangesAsync(cancellationToken);
        return DataResult<BankCurrency>.Success(bankCurrency);
    }
}
