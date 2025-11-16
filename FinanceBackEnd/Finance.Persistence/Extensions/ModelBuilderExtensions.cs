using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Incomes;
using Finance.Domain.Models.IOLInvestments;
using Finance.Domain.Models.Movements;
using Microsoft.EntityFrameworkCore;

namespace Finance.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Configures global query filters for all entities, including ownership filters that restrict access to user-owned resources
    /// and related entities through their permission associations.
    /// </summary>
    public static void AddQueryFilters(this ModelBuilder modelBuilder, FinanceDbContext context)
    {
        SetEntityOwnershipFilter<CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>(modelBuilder, context);
        SetEntityOwnershipFilter<DebitOrigin, Guid, DebitOriginPermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<DebitOrigin, Guid, DebitOriginPermissions>(modelBuilder, context);
        SetEntityOwnershipFilter<Debit, Guid, DebitPermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<Debit, Guid, DebitPermissions>(modelBuilder, context);
        SetEntityOwnershipFilter<Fund, Guid, FundPermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<Fund, Guid, FundPermissions>(modelBuilder, context);
        SetEntityOwnershipFilter<Income, Guid, IncomePermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<Income, Guid, IncomePermissions>(modelBuilder, context);
        SetEntityOwnershipFilter<IOLInvestmentAsset, Guid, IOLInvestmentAssetPermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<IOLInvestmentAsset, Guid, IOLInvestmentAssetPermissions>(modelBuilder, context);
        SetEntityOwnershipFilter<IOLInvestment, Guid, IOLInvestmentPermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<IOLInvestment, Guid, IOLInvestmentPermissions>(modelBuilder, context);
        SetEntityOwnershipFilter<Movement, Guid, MovementPermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<Movement, Guid, MovementPermissions>(modelBuilder, context);
        SetCurrencyConversionFilter(modelBuilder, context);
        SetEntityOwnershipFilter<CreditCard, Guid, CreditCardPermissions>(modelBuilder, context);
        SetPermissionsOwnershipFilter<CreditCard, Guid, CreditCardPermissions>(modelBuilder, context);
        SetCreditCardRelatedEntitiesFilter(modelBuilder, context);
    }

    /// <summary>
    /// Registers a global ownership query filter for the given entity type, restricting access to entities owned by the current user.
    /// </summary>
    private static void SetEntityOwnershipFilter<TEntity, TId, TResourcePermissions>(ModelBuilder modelBuilder, FinanceDbContext context)
        where TEntity : Entity<TId>, new()
        where TResourcePermissions : ResourcePermissions<TEntity, TId>
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => context.Set<TResourcePermissions>()
            .Any(p => p.ResourceId!.Equals(e.Id) && p.User.Identities.Any(i => i.SourceId == context.CurrentUserId)));
    }

    /// <summary>
    /// Registers a global ownership query filter for the resource permissions entity, restricting access to permissions owned by the current user.
    /// </summary>
    private static void SetPermissionsOwnershipFilter<TEntity, TId, TResourcePermissions>(ModelBuilder modelBuilder, FinanceDbContext context)
        where TEntity : Entity<TId>, new()
        where TResourcePermissions : ResourcePermissions<TEntity, TId>
    {
        modelBuilder.Entity<TResourcePermissions>().HasQueryFilter(r => r.User.Identities.Any(i => i.SourceId == context.CurrentUserId));
    }

    /// <summary>
    /// Registers a query filter for CurrencyConversion entities to exclude records with null Movement or Currency references.
    /// </summary>
    private static void SetCurrencyConversionFilter(ModelBuilder modelBuilder, FinanceDbContext context)
    {
        modelBuilder.Entity<CurrencyConversion>().HasQueryFilter(cc => cc.Movement != null && cc.Currency != null);
    }

    /// <summary>
    /// Sets ownership filters for credit card related entities through their navigation properties to CreditCard.
    /// This ensures users can only see statements, transactions, and payments for credit cards they own.
    /// </summary>
    private static void SetCreditCardRelatedEntitiesFilter(ModelBuilder modelBuilder, FinanceDbContext context)
    {
        modelBuilder.Entity<CreditCardStatement>().HasQueryFilter(
            s => context.Set<CreditCardPermissions>().Any(r =>
                r.ResourceId == s.CreditCardId &&
                r.User.Identities.Any(i => i.SourceId == context.CurrentUserId)
            )
        );

        modelBuilder.Entity<CreditCardTransaction>().HasQueryFilter(
            t => context.Set<CreditCardPermissions>().Any(r =>
                r.ResourceId == t.CreditCardId &&
                r.User.Identities.Any(i => i.SourceId == context.CurrentUserId)
            )
        );

        modelBuilder.Entity<CreditCardStatementTransaction>().HasQueryFilter(
            st => context.Set<CreditCardStatement>()
                .Where(s => s.Id == st.CreditCardStatementId)
                .Join(context.Set<CreditCardPermissions>(),
                    s => s.CreditCardId,
                    r => r.ResourceId,
                    (s, r) => r)
                .Any(r => r.User.Identities.Any(i => i.SourceId == context.CurrentUserId))
        );

        modelBuilder.Entity<CreditCardPayment>().HasQueryFilter(
            p => context.Set<CreditCardStatement>()
                .Where(s => s.Id == p.StatementId)
                .Join(context.Set<CreditCardPermissions>(),
                    s => s.CreditCardId,
                    r => r.ResourceId,
                    (s, r) => r)
                .Any(r => r.User.Identities.Any(i => i.SourceId == context.CurrentUserId))
        );

        modelBuilder.Entity<CreditCardStatementAdjustment>().HasQueryFilter(
            a => context.Set<CreditCardStatement>()
                .Where(s => s.Id == a.CreditCardStatementId)
                .Join(context.Set<CreditCardPermissions>(),
                    s => s.CreditCardId,
                    r => r.ResourceId,
                    (s, r) => r)
                .Any(r => r.User.Identities.Any(i => i.SourceId == context.CurrentUserId))
        );
    }
}
