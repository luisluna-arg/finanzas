using Finance.Domain.Models.Auth;
using Finance.Domain.Models.BankCurrencies;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.BankCurrencies;

/// <summary>
/// Funds carry a (BankId, CurrencyId) FK into BankCurrency, so logging a Fund for a pair that has
/// never been configured needs a BankCurrency row to exist first. This creates one on the fly,
/// defaulted to DailyUse = false, and grants the acting user ownership of it.
/// </summary>
internal static class BankCurrencyProvisioning
{
    public static async Task<BankCurrency> EnsureExistsAsync(FinanceDbContext db, Guid bankId, Guid currencyId, Guid userId, CancellationToken cancellationToken)
    {
        var existing = await db.BankCurrency
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.BankId == bankId && o.CurrencyId == currencyId, cancellationToken);

        if (existing != null) return existing;

        var bankCurrency = new BankCurrency { BankId = bankId, CurrencyId = currencyId, DailyUse = false };
        db.BankCurrency.Add(bankCurrency);
        db.BankCurrencyPermissions.Add(new BankCurrencyPermissions
        {
            BankId = bankId,
            CurrencyId = currencyId,
            UserId = userId,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });

        return bankCurrency;
    }
}
