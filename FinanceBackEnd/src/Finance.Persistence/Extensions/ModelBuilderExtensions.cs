using System.Linq.Expressions;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Identities;
using Finance.Domain.Models.Incomes;
using Finance.Domain.Models.IOLInvestments;
using Finance.Domain.Models.Movements;
using Finance.Domain.Models.Subscriptions;
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
        ApplyEntityOwnershipFilter<CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>(modelBuilder, context);
        ApplyEntityOwnershipFilter<DebitOrigin, Guid, DebitOriginPermissions>(modelBuilder, context);
        ApplyEntityOwnershipFilter<Debit, Guid, DebitPermissions>(modelBuilder, context);
        ApplyEntityOwnershipFilter<Fund, Guid, FundPermissions>(modelBuilder, context);
        ApplyEntityOwnershipFilter<Income, Guid, IncomePermissions>(modelBuilder, context);
        ApplyEntityOwnershipFilter<IOLInvestmentAsset, Guid, IOLInvestmentAssetPermissions>(modelBuilder, context);
        ApplyEntityOwnershipFilter<IOLInvestment, Guid, IOLInvestmentPermissions>(modelBuilder, context);
        ApplyEntityOwnershipFilter<Movement, Guid, MovementPermissions>(modelBuilder, context);
        ApplyEntityOwnershipFilter<Subscription, Guid, SubscriptionPermissions>(modelBuilder, context);
        ApplyCurrencyConversionFilter(modelBuilder, context);
        ApplyCreditCardRelatedEntitiesFilter(modelBuilder, context);
    }

    /// <summary>
    /// Registers a global ownership query filter for the given entity type, restricting access to entities owned by the current user.
    /// </summary>
    private static void ApplyEntityOwnershipFilter<TEntity, TId, TResourcePermissions>(ModelBuilder modelBuilder, FinanceDbContext context)
        where TEntity : Entity<TId>, new()
        where TResourcePermissions : ResourcePermissions<TEntity, TId>
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => context.Set<TResourcePermissions>()
            .Any(p => p.ResourceId!.Equals(e.Id) && p.User.Identities.Any(i => i.SourceId == context.CurrentUserId)));
        // Also restrict the permissions set to the current user to keep filters consistent
        ApplyPermissionFilter<TEntity, TId, TResourcePermissions>(modelBuilder, context);
    }

    /// <summary>
    /// Registers a query filter for CurrencyConversion entities to exclude records with null Movement or Currency references.
    /// </summary>
    private static void ApplyCurrencyConversionFilter(ModelBuilder modelBuilder, FinanceDbContext context)
    {
        modelBuilder.Entity<CurrencyConversion>().HasQueryFilter(cc => cc.Movement != null && cc.Currency != null);
    }

    /// <summary>
    /// Sets ownership filters for credit card related entities through their navigation properties to CreditCard.
    /// This ensures users can only see statements, transactions, and payments for credit cards they own.
    /// </summary>
    private static void ApplyCreditCardRelatedEntitiesFilter(ModelBuilder modelBuilder, FinanceDbContext context)
    {
        ApplyEntityOwnershipFilter<CreditCard, Guid, CreditCardPermissions>(modelBuilder, context);

        ApplyCreditCardEntityFilter<CreditCardStatement>(context, modelBuilder);
        ApplyCreditCardEntityFilter<CreditCardTransaction>(context, modelBuilder);

        ApplyCreditCardStatementEntityFilter<CreditCardStatementTransaction>(context, modelBuilder);
        ApplyCreditCardStatementEntityFilter<CreditCardPayment>(context, modelBuilder);
        ApplyCreditCardStatementEntityFilter<CreditCardStatementAdjustment>(context, modelBuilder);
    }

    /// <summary>
    /// Restricts entities to those whose <c>CreditCard</c> is owned by the current user.
    /// </summary>
    /// <typeparam name="T">Credit-card-related entity type.</typeparam>
    /// <param name="context">Current <see cref="FinanceDbContext"/>.</param>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to configure.</param>
    private static void ApplyCreditCardEntityFilter<T>(FinanceDbContext context, ModelBuilder modelBuilder)
        where T : CreditCardEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(
            s => context.Set<CreditCardPermissions>()
                .Join(context.Set<User>(),
                    permission => permission.UserId,
                    user => user.Id,
                    (permission, user) => new { permission, user })
                .Where(x => x.permission.ResourceId == s.CreditCardId)
                .SelectMany(x => x.user.Identities)
                .Any(i => i.SourceId == context.CurrentUserId));
    }

    /// <summary>
    /// Restricts statement-related entities to those whose statement's credit card
    /// is owned by the current user.
    /// </summary>
    /// <typeparam name="T">Statement-related entity type.</typeparam>
    /// <param name="context">Current <see cref="FinanceDbContext"/>.</param>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to configure.</param>
    private static void ApplyCreditCardStatementEntityFilter<T>(FinanceDbContext context, ModelBuilder modelBuilder)
        where T : CreditCardStatementEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(
            st => context.Set<CreditCardStatement>()
                .Where(s => s.Id == st.StatementId)
                .Join(context.Set<CreditCardPermissions>(),
                    s => s.CreditCardId,
                    p => p.ResourceId,
                    (s, p) => p)
                .Any(p => p.User.Identities.Any(i => i.SourceId == context.CurrentUserId)));
    }

    /// <summary>
    /// Builds and applies an ownership filter for <typeparamref name="TSource"/> using
    /// <typeparamref name="TSourcePermissions"/> to match resource ids and user identities.
    /// </summary>
    /// <typeparam name="TSource">Target entity type.</typeparam>
    /// <typeparam name="TId">Entity id type.</typeparam>
    /// <typeparam name="TSourcePermissions">Permissions/join type used for ownership checks.</typeparam>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to register the filter.</param>
    /// <param name="dbContext">The <see cref="FinanceDbContext"/> providing the current user id.</param>
    private static void ApplyOwnerShipFilter<TSource, TId, TSourcePermissions>(ModelBuilder modelBuilder, FinanceDbContext dbContext)
        where TSource : Entity<TId>, new()
        where TSourcePermissions : ResourcePermissions<TSource, TId>
    {
        var entityParam = Expression.Parameter(typeof(TSource), "e");
        var joinParam = Expression.Parameter(typeof(TSourcePermissions), "j");

        var joinKeyProp = Expression.Property(joinParam, "ResourceId");
        var entityKeyProp = Expression.Property(entityParam, nameof(Entity<TId>.Id));
        var keyEqualsExpr = Expression.Equal(joinKeyProp, entityKeyProp);

        var identityAnyExpr = BuildIdentityAnyExpr<TSource, TId, TSourcePermissions>(joinParam, dbContext);

        var combinedBody = Expression.AndAlso(identityAnyExpr, keyEqualsExpr);
        var joinPredicate = Expression.Lambda(combinedBody, joinParam);

        var setCall = Expression.Call(Expression.Constant(dbContext), nameof(DbContext.Set), [typeof(TSourcePermissions)]);

        var anyMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TSourcePermissions));

        var anyCall = Expression.Call(anyMethod, setCall, joinPredicate);

        var lambda = Expression.Lambda<Func<TSource, bool>>(anyCall, entityParam);
        modelBuilder.Entity<TSource>().HasQueryFilter(lambda);
    }

    /// <summary>
    /// Restricts permission records to those belonging to the current user.
    /// </summary>
    /// <typeparam name="TEntity">Referenced entity type.</typeparam>
    /// <typeparam name="TId">Referenced entity id type.</typeparam>
    /// <typeparam name="TPermissions">Permission entity type.</typeparam>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to configure.</param>
    /// <param name="dbContext">The <see cref="FinanceDbContext"/> providing the current user id.</param>
    private static void ApplyPermissionFilter<TEntity, TId, TPermissions>(ModelBuilder modelBuilder, FinanceDbContext dbContext)
        where TEntity : Entity<TId>, new()
        where TPermissions : ResourcePermissions<TEntity, TId>
    {
        var permissionParam = Expression.Parameter(typeof(TPermissions), "p");
        var identityAnyExpr = BuildIdentityAnyExpr<TEntity, TId, TPermissions>(permissionParam, dbContext);
        var lambda = Expression.Lambda<Func<TPermissions, bool>>(identityAnyExpr, permissionParam);
        modelBuilder.Entity<TPermissions>().HasQueryFilter(lambda);
    }

    /// <summary>
    /// Builds an expression that checks if <c>j.User.Identities</c> contains the current user identity.
    /// </summary>
    /// <typeparam name="TEntity">Referenced entity type.</typeparam>
    /// <typeparam name="TKey">Entity id type.</typeparam>
    /// <typeparam name="TJoin">Permission/join type.</typeparam>
    /// <param name="joinParam">Parameter expression for the join instance.</param>
    /// <param name="ctx">The <see cref="FinanceDbContext"/> providing the current user id.</param>
    /// <returns>An <see cref="Expression"/> for the identity-any check.</returns>
    private static Expression BuildIdentityAnyExpr<TEntity, TKey, TJoin>(
        ParameterExpression joinParam,
        FinanceDbContext ctx)
        where TEntity : Entity<TKey>, new()
        where TJoin : ResourcePermissions<TEntity, TKey>
    {
        var userProp = Expression.Property(joinParam, nameof(ResourcePermissions<TEntity, TKey>.User));
        var userIdentitiesProp = Expression.Property(userProp, nameof(User.Identities));

        var identParam = Expression.Parameter(typeof(Identity), "i");
        var sourceIdProp = Expression.Property(identParam, nameof(Identity.SourceId));
        var currentUserId = Expression.Property(Expression.Constant(ctx), nameof(FinanceDbContext.CurrentUserId));
        var idEquals = Expression.Equal(sourceIdProp, currentUserId);
        var identLambda = Expression.Lambda(idEquals, identParam);

        var anyIdentityMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(Identity));

        var asQueryableMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethodDefinition)
            .MakeGenericMethod(typeof(Identity));
        var identitiesQueryable = Expression.Call(asQueryableMethod, userIdentitiesProp);

        var anyIdentityCall = Expression.Call(anyIdentityMethod, identitiesQueryable, identLambda);

        return anyIdentityCall;
    }

}
