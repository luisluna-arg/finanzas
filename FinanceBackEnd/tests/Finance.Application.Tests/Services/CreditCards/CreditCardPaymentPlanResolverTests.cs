using Finance.Application.Services.CreditCards;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.CreditCards;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Tests.Services.CreditCards;

public class CreditCardPaymentPlanResolverTests : QueryHandlerBaseTests
{
    private const string ParensPattern = @"^(?<base>.*?)\s*\(\s*(?<n>\d{1,2})\s*/\s*(?<m>\d{1,2})\s*\)\s*$";

    private CreditCardPaymentPlanResolver CreateResolver() => new(_dbContext);

    private async Task<(Guid CreditCardId, Guid TemplateId)> SeedCardWithPatternAsync(string regex = ParensPattern)
    {
        var creditCard = new CreditCard { Id = Guid.NewGuid(), Name = "Visa" };
        _dbContext.CreditCard.Add(creditCard);

        var pattern = new CreditCardInstallmentPattern
        {
            Id = Guid.NewGuid(),
            Name = "Test pattern",
            RegexPattern = regex,
            IsSystem = true,
        };
        _dbContext.CreditCardInstallmentPattern.Add(pattern);

        var template = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Test template",
            IsSystem = true,
            ConfigJson = "{}",
            InstallmentPatternId = pattern.Id,
        };
        _dbContext.CreditCardStatementImportTemplate.Add(template);

        await _dbContext.SaveChangesAsync();

        return (creditCard.Id, template.Id);
    }

    [Fact]
    public void ParseConcept_NoMatchingPatterns_ReturnsSingleInstallment()
    {
        var resolver = CreateResolver();

        var parsed = resolver.ParseConcept("Netflix", []);

        Assert.Equal("Netflix", parsed.BaseConcept);
        Assert.Equal(1, parsed.InstallmentNumber);
        Assert.Equal(1, parsed.TotalInstallments);
    }

    [Fact]
    public void ParseConcept_MatchingPattern_ExtractsBaseAndInstallmentNumbers()
    {
        var resolver = CreateResolver();
        var regex = new System.Text.RegularExpressions.Regex(ParensPattern);

        var parsed = resolver.ParseConcept("MERPAGO*MERCADOLIBRE (2/3)", [regex]);

        Assert.Equal("MERPAGO*MERCADOLIBRE", parsed.BaseConcept);
        Assert.Equal(2, parsed.InstallmentNumber);
        Assert.Equal(3, parsed.TotalInstallments);
    }

    [Fact]
    public async Task ResolvePlanAsync_SequentialInstallments_ShareSamePlan()
    {
        var (creditCardId, templateId) = await SeedCardWithPatternAsync();
        var resolver = CreateResolver();
        var currencyId = Guid.NewGuid();

        var (plan1, n1) = await resolver.ResolvePlanAsync(
            creditCardId, currencyId, "Amazon (1/3)", CreditCardTransactionType.Purchase, templateId, default);
        _dbContext.CreditCardTransaction.Add(new CreditCardTransaction
        {
            CreditCardId = creditCardId,
            CurrencyId = currencyId,
            Concept = "Amazon (1/3)",
            Amount = 100m,
            Timestamp = DateTime.UtcNow,
            TransactionType = CreditCardTransactionType.Purchase,
            PaymentPlanId = plan1,
            InstallmentNumber = n1,
        });
        await _dbContext.SaveChangesAsync();

        var (plan2, n2) = await resolver.ResolvePlanAsync(
            creditCardId, currencyId, "Amazon (2/3)", CreditCardTransactionType.Purchase, templateId, default);

        Assert.Equal(plan1, plan2);
        Assert.Equal(1, n1);
        Assert.Equal(2, n2);
    }

    [Fact]
    public async Task ResolvePlanAsync_FirstSeenInstallmentIsNotOne_CreatesPlanLeavingGap()
    {
        var (creditCardId, templateId) = await SeedCardWithPatternAsync();
        var resolver = CreateResolver();
        var currencyId = Guid.NewGuid();

        var (planId, installmentNumber) = await resolver.ResolvePlanAsync(
            creditCardId, currencyId, "Amazon (2/3)", CreditCardTransactionType.Purchase, templateId, default);
        await _dbContext.SaveChangesAsync();

        Assert.Equal(2, installmentNumber);

        var plan = await _dbContext.CreditCardPaymentPlan.IgnoreQueryFilters()
            .SingleAsync(p => p.Id == planId);
        Assert.Equal(3, plan.TotalInstallments);
        Assert.Equal("Amazon", plan.BaseConcept);
    }

    [Fact]
    public async Task ResolvePlanAsync_PlanOfOne_IsNeverReused()
    {
        var (creditCardId, templateId) = await SeedCardWithPatternAsync();
        var resolver = CreateResolver();
        var currencyId = Guid.NewGuid();

        var (plan1, _) = await resolver.ResolvePlanAsync(
            creditCardId, currencyId, "Netflix", CreditCardTransactionType.Purchase, templateId, default);
        var (plan2, _) = await resolver.ResolvePlanAsync(
            creditCardId, currencyId, "Netflix", CreditCardTransactionType.Purchase, templateId, default);

        Assert.NotEqual(plan1, plan2);
    }

    [Fact]
    public async Task ResolvePlanAsync_DifferentCurrency_DoesNotMatchExistingPlan()
    {
        var (creditCardId, templateId) = await SeedCardWithPatternAsync();
        var resolver = CreateResolver();

        var (plan1, _) = await resolver.ResolvePlanAsync(
            creditCardId, Guid.NewGuid(), "Amazon (1/3)", CreditCardTransactionType.Purchase, templateId, default);
        var (plan2, _) = await resolver.ResolvePlanAsync(
            creditCardId, Guid.NewGuid(), "Amazon (2/3)", CreditCardTransactionType.Purchase, templateId, default);

        Assert.NotEqual(plan1, plan2);
    }

    [Fact]
    public async Task ResolvePlanAsync_NonPurchaseType_AlwaysCreatesFreshPlan()
    {
        var (creditCardId, templateId) = await SeedCardWithPatternAsync();
        var resolver = CreateResolver();
        var currencyId = Guid.NewGuid();

        var (purchasePlan, _) = await resolver.ResolvePlanAsync(
            creditCardId, currencyId, "Amazon (1/3)", CreditCardTransactionType.Purchase, templateId, default);
        var (refundPlan, refundInstallment) = await resolver.ResolvePlanAsync(
            creditCardId, currencyId, "Amazon (1/3)", CreditCardTransactionType.Refund, templateId, default);

        Assert.NotEqual(purchasePlan, refundPlan);
        Assert.Equal(1, refundInstallment);
    }

    [Fact]
    public async Task ResolvePlanAsync_TieBreak_PicksOldestOpenPlan()
    {
        var (creditCardId, templateId) = await SeedCardWithPatternAsync();
        var resolver = CreateResolver();
        var currencyId = Guid.NewGuid();

        var olderPlan = new CreditCardPaymentPlan
        {
            Id = Guid.NewGuid(),
            CreditCardId = creditCardId,
            CurrencyId = currencyId,
            BaseConcept = "Amazon",
            TotalInstallments = 3,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
        };
        var newerPlan = new CreditCardPaymentPlan
        {
            Id = Guid.NewGuid(),
            CreditCardId = creditCardId,
            CurrencyId = currencyId,
            BaseConcept = "Amazon",
            TotalInstallments = 3,
            CreatedAt = DateTime.UtcNow,
        };
        _dbContext.CreditCardPaymentPlan.AddRange(olderPlan, newerPlan);
        await _dbContext.SaveChangesAsync();

        var (resolvedPlanId, installmentNumber) = await resolver.ResolvePlanAsync(
            creditCardId, currencyId, "Amazon (1/3)", CreditCardTransactionType.Purchase, templateId, default);

        Assert.Equal(olderPlan.Id, resolvedPlanId);
        Assert.Equal(1, installmentNumber);
    }
}
