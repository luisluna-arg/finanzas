using System.Text.RegularExpressions;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Services.CreditCards;

/// <summary>
/// Parses installment markers (e.g. "(2/3)") out of a transaction's concept using the credit card's
/// configured patterns, and finds or creates the CreditCardPaymentPlan it continues. Shared by manual
/// transaction create/update, statement import, and the historical backfill so all three resolve
/// installments identically.
/// </summary>
public class CreditCardPaymentPlanResolver(FinanceDbContext db)
{
    public record ParsedConcept(string BaseConcept, int InstallmentNumber, int TotalInstallments);

    public async Task<(Guid PaymentPlanId, int InstallmentNumber)> ResolvePlanAsync(
        Guid creditCardId,
        Guid currencyId,
        string concept,
        CreditCardTransactionType transactionType,
        Guid? preferredTemplateId,
        CancellationToken cancellationToken,
        Guid? excludeTransactionId = null)
    {
        if (transactionType != CreditCardTransactionType.Purchase)
        {
            var plan = CreateNewPlan(creditCardId, currencyId, Normalize(concept), 1);
            return (plan.Id, 1);
        }

        var patterns = await GetCandidatePatternsAsync(creditCardId, preferredTemplateId, cancellationToken);
        var parsed = ParseConcept(concept, patterns);

        if (parsed.TotalInstallments == 1)
        {
            var plan = CreateNewPlan(creditCardId, currencyId, parsed.BaseConcept, 1);
            return (plan.Id, 1);
        }

        var openPlan = await FindOpenPlanAsync(
            creditCardId, currencyId, parsed.BaseConcept, parsed.TotalInstallments, parsed.InstallmentNumber, excludeTransactionId, cancellationToken);

        if (openPlan != null) return (openPlan.Id, parsed.InstallmentNumber);

        var newPlan = CreateNewPlan(creditCardId, currencyId, parsed.BaseConcept, parsed.TotalInstallments);
        return (newPlan.Id, parsed.InstallmentNumber);
    }

    public ParsedConcept ParseConcept(string concept, IEnumerable<Regex> candidatePatterns)
    {
        var safeConcept = concept ?? string.Empty;

        foreach (var pattern in candidatePatterns)
        {
            var match = pattern.Match(safeConcept);
            if (!match.Success || !match.Groups["n"].Success || !match.Groups["m"].Success) continue;
            if (!int.TryParse(match.Groups["n"].Value, out var n) || n <= 0) continue;
            if (!int.TryParse(match.Groups["m"].Value, out var m) || m <= 0) continue;

            var baseText = match.Groups["base"].Success ? match.Groups["base"].Value : safeConcept;
            return new ParsedConcept(Normalize(baseText), n, m);
        }

        return new ParsedConcept(Normalize(safeConcept), 1, 1);
    }

    private async Task<List<Regex>> GetCandidatePatternsAsync(Guid creditCardId, Guid? preferredTemplateId, CancellationToken ct)
    {
        var patternIds = new List<Guid>();

        if (preferredTemplateId.HasValue)
        {
            var preferred = await GetTemplatePatternIdAsync(preferredTemplateId.Value, ct);
            if (preferred.HasValue) patternIds.Add(preferred.Value);
        }

        var card = await db.CreditCard
            .IgnoreQueryFilters()
            .Where(c => c.Id == creditCardId)
            .Select(c => c.DefaultImportTemplateId)
            .FirstOrDefaultAsync(ct);

        if (card.HasValue)
        {
            var defaultPatternId = await GetTemplatePatternIdAsync(card.Value, ct);
            if (defaultPatternId.HasValue && !patternIds.Contains(defaultPatternId.Value))
                patternIds.Add(defaultPatternId.Value);
        }

        var linkedPatternIds = await db.CreditCardStatementImportTemplate
            .IgnoreQueryFilters()
            .Where(t => t.InstallmentPatternId != null && t.CreditCards.Any(c => c.Id == creditCardId))
            .Select(t => t.InstallmentPatternId!.Value)
            .Distinct()
            .ToListAsync(ct);

        foreach (var id in linkedPatternIds)
        {
            if (!patternIds.Contains(id)) patternIds.Add(id);
        }

        if (patternIds.Count == 0) return [];

        var regexByPatternId = await db.CreditCardInstallmentPattern
            .IgnoreQueryFilters()
            .Where(p => patternIds.Contains(p.Id) && !p.Deactivated)
            .Select(p => new { p.Id, p.RegexPattern })
            .ToDictionaryAsync(p => p.Id, p => p.RegexPattern, ct);

        return patternIds
            .Where(regexByPatternId.ContainsKey)
            .Select(id => new Regex(regexByPatternId[id], RegexOptions.IgnoreCase))
            .ToList();
    }

    private async Task<Guid?> GetTemplatePatternIdAsync(Guid templateId, CancellationToken ct)
    {
        var patternId = await db.CreditCardStatementImportTemplate
            .IgnoreQueryFilters()
            .Where(t => t.Id == templateId)
            .Select(t => t.InstallmentPatternId)
            .FirstOrDefaultAsync(ct);

        return patternId;
    }

    private async Task<CreditCardPaymentPlan?> FindOpenPlanAsync(
        Guid creditCardId, Guid currencyId, string baseConcept, int totalInstallments, int installmentNumber,
        Guid? excludeTransactionId, CancellationToken ct)
    {
        var dbCandidates = await db.CreditCardPaymentPlan
            .IgnoreQueryFilters()
            .Where(p => p.CreditCardId == creditCardId
                && p.CurrencyId == currencyId
                && p.TotalInstallments == totalInstallments
                && !p.Deactivated
                && p.BaseConcept.ToLower() == baseConcept.ToLower())
            .ToListAsync(ct);

        var trackedCandidates = db.ChangeTracker.Entries<CreditCardPaymentPlan>()
            .Where(e => e.State == EntityState.Added
                && e.Entity.CreditCardId == creditCardId
                && e.Entity.CurrencyId == currencyId
                && e.Entity.TotalInstallments == totalInstallments
                && string.Equals(e.Entity.BaseConcept, baseConcept, StringComparison.OrdinalIgnoreCase))
            .Select(e => e.Entity);

        var candidates = dbCandidates
            .Concat(trackedCandidates)
            .DistinctBy(p => p.Id)
            .OrderBy(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .ToList();

        foreach (var candidate in candidates)
        {
            if (!await IsSlotTakenAsync(candidate.Id, installmentNumber, excludeTransactionId, ct))
                return candidate;
        }

        return null;
    }

    private async Task<bool> IsSlotTakenAsync(Guid planId, int installmentNumber, Guid? excludeTransactionId, CancellationToken ct)
    {
        var takenInDb = await db.CreditCardTransaction
            .IgnoreQueryFilters()
            .AnyAsync(t => t.PaymentPlanId == planId && t.InstallmentNumber == installmentNumber && !t.Deactivated
                && t.Id != excludeTransactionId, ct);

        if (takenInDb) return true;

        return db.ChangeTracker.Entries<CreditCardTransaction>()
            .Any(e => e.State == EntityState.Added
                && e.Entity.PaymentPlanId == planId
                && e.Entity.InstallmentNumber == installmentNumber
                && !e.Entity.Deactivated);
    }

    private CreditCardPaymentPlan CreateNewPlan(Guid creditCardId, Guid currencyId, string baseConcept, int totalInstallments)
    {
        var plan = new CreditCardPaymentPlan
        {
            CreditCardId = creditCardId,
            CurrencyId = currencyId,
            BaseConcept = baseConcept,
            TotalInstallments = totalInstallments,
        };
        db.CreditCardPaymentPlan.Add(plan);
        return plan;
    }

    private static string Normalize(string? text) => Regex.Replace(text ?? string.Empty, @"\s+", " ").Trim();
}
