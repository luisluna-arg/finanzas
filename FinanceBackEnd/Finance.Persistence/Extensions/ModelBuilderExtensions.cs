using System.Linq.Expressions;
using Finance.Domain.Models;
using Finance.Domain.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Finance.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds global query filters to the model, such as user-ownership filters for entities like Fund.
    /// </summary>
    public static void AddQueryFilters(this ModelBuilder modelBuilder, FinanceDbContext context)
    {
        SetOwnershipFilter<CurrencyExchangeRate, Guid, CurrencyExchangeRateResource>(modelBuilder, context);
        SetResourceOwnershipFilter<CurrencyExchangeRate, Guid, CurrencyExchangeRateResource>(modelBuilder, context);
        SetOwnershipFilter<DebitOrigin, Guid, DebitOriginResource>(modelBuilder, context);
        SetResourceOwnershipFilter<DebitOrigin, Guid, DebitOriginResource>(modelBuilder, context);
        SetOwnershipFilter<Debit, Guid, DebitResource>(modelBuilder, context);
        SetResourceOwnershipFilter<Debit, Guid, DebitResource>(modelBuilder, context);
        SetOwnershipFilter<Fund, Guid, FundResource>(modelBuilder, context);
        SetResourceOwnershipFilter<Fund, Guid, FundResource>(modelBuilder, context);
        SetOwnershipFilter<Income, Guid, IncomeResource>(modelBuilder, context);
        SetResourceOwnershipFilter<Income, Guid, IncomeResource>(modelBuilder, context);
        SetOwnershipFilter<IOLInvestmentAsset, Guid, IOLInvestmentAssetResource>(modelBuilder, context);
        SetResourceOwnershipFilter<IOLInvestmentAsset, Guid, IOLInvestmentAssetResource>(modelBuilder, context);
        SetOwnershipFilter<IOLInvestment, Guid, IOLInvestmentResource>(modelBuilder, context);
        SetResourceOwnershipFilter<IOLInvestment, Guid, IOLInvestmentResource>(modelBuilder, context);
        SetOwnershipFilter<Movement, Guid, MovementResource>(modelBuilder, context);
        SetResourceOwnershipFilter<Movement, Guid, MovementResource>(modelBuilder, context);
        SetCurrencyConversionOwnershipFilter(modelBuilder, context);
        // Credit card related entities filtered through CreditCard ownership
        SetOwnershipFilter<CreditCard, Guid, CreditCardResource>(modelBuilder, context);
        SetResourceOwnershipFilter<CreditCard, Guid, CreditCardResource>(modelBuilder, context);
        SetCreditCardRelatedOwnershipFilter(modelBuilder, context);
    }

    /// <summary>
    /// Registers a global ownership query filter for the given entity type, restricting access to entities owned by the current user.
    /// </summary>
    private static void SetOwnershipFilter<TEntity, TId, TEntityResource>(ModelBuilder modelBuilder, FinanceDbContext context)
        where TEntity : Entity<TId>, new()
        where TEntityResource : EntityResource<TEntity, TId>
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(BuildOwnerShipFilter<TEntity, TId, TEntityResource>(context));
    }

    /// <summary>
    /// Builds a query filter expression for an entity, restricting access to entities joined to resources owned by the current user.
    /// </summary>
    private static LambdaExpression BuildOwnerShipFilter<TEntity, TId, TEntityResource>(FinanceDbContext dbContext)
        where TEntity : Entity<TId>, new()
        where TEntityResource : EntityResource<TEntity, TId>
    {
        // 1) e =>
        var entityParam = Expression.Parameter(typeof(TEntity), "e");
        // 2) j =>
        var joinParam = Expression.Parameter(typeof(TEntityResource), "j");

        // j.ResourceSourceId
        var joinKeyProp = Expression.Property(joinParam, nameof(EntityResource<TEntity, TId>.ResourceSourceId));
        // e.Id
        var entityKeyProp = Expression.Property(entityParam, nameof(Entity<TId>.Id));
        // build: (j.ResourceSourceId == e.Id)
        var keyEqualsExpr = Expression.Equal(joinKeyProp, entityKeyProp);

        // build: j.Resource.User.Identities.Any(i => i.SourceId == ctx.CurrentUserId)
        var identityAnyExpr = BuildIdentityAnyExpr<TEntity, TId, TEntityResource>(joinParam, dbContext);

        // combine: j => identityAny && keyEquals
        var combinedBody = Expression.AndAlso(identityAnyExpr, keyEqualsExpr);
        var joinPredicate = Expression.Lambda(combinedBody, joinParam);

        // call: ctx.Set<TJoin>().Any(j => …)
        var setCall = Expression.Call(
            Expression.Constant(dbContext),
            nameof(DbContext.Set),
            [typeof(TEntityResource)]
        );

        var anyMethod = typeof(Queryable)
            .GetMethods()
            .Single(m =>
                m.Name == nameof(Queryable.Any) &&
                m.GetParameters().Length == 2
            )
            .MakeGenericMethod(typeof(TEntityResource));

        var anyCall = Expression.Call(anyMethod, setCall, joinPredicate);

        // final lambda: e => ctx.Set<TJoin>().Any(j => …)
        return Expression.Lambda(anyCall, entityParam);
    }

    /// <summary>
    /// Builds an expression that checks if any ResourceOwner for the given join entity's resource 
    /// is linked to the current user (by identity).
    /// </summary>
    private static Expression BuildIdentityAnyExpr<TEntity, TKey, TJoin>(
        ParameterExpression joinParam,
        FinanceDbContext ctx)
        where TEntity : Entity<TKey>, new()
        where TJoin : EntityResource<TEntity, TKey>
    {
        // j.ResourceId
        var resourceIdProp = Expression.Property(joinParam, nameof(EntityResource<TEntity, TKey>.ResourceId));

        // ResourceOwner ro where ro.ResourceId == j.ResourceId
        var resourceOwnerParam = Expression.Parameter(typeof(ResourceOwner), "ro");
        var roResourceIdProp = Expression.Property(resourceOwnerParam, nameof(ResourceOwner.ResourceId));
        var roUserProp = Expression.Property(resourceOwnerParam, nameof(ResourceOwner.User));
        var roUserIdentitiesProp = Expression.Property(roUserProp, nameof(User.Identities));

        // i => i.SourceId == ctx.CurrentUserId
        var identParam = Expression.Parameter(typeof(Identity), "i");
        var sourceIdProp = Expression.Property(identParam, nameof(Identity.SourceId));
        var currentUserId = Expression.Property(Expression.Constant(ctx), nameof(FinanceDbContext.CurrentUserId));
        var idEquals = Expression.Equal(sourceIdProp, currentUserId);
        var identLambda = Expression.Lambda(idEquals, identParam);

        // ro.User.Identities.AsQueryable().Any(i => ...)
        var asQueryableMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethodDefinition)
            .MakeGenericMethod(typeof(Identity));
        var identitiesQueryable = Expression.Call(asQueryableMethod, roUserIdentitiesProp);
        var anyIdentityMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(Identity));
        var anyIdentityCall = Expression.Call(anyIdentityMethod, identitiesQueryable, identLambda);

        // ro => ro.ResourceId == j.ResourceId && ro.User.Identities.Any(...)
        var roIdEquals = Expression.Equal(roResourceIdProp, resourceIdProp);
        var roBody = Expression.AndAlso(roIdEquals, anyIdentityCall);
        var roLambda = Expression.Lambda(roBody, resourceOwnerParam);

        // ctx.Set<ResourceOwner>().Any(ro => ...)
        var setCall = Expression.Call(
            Expression.Constant(ctx),
            nameof(DbContext.Set),
            [typeof(ResourceOwner)]
        );
        var anyResourceOwnerMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(ResourceOwner));
        return Expression.Call(anyResourceOwnerMethod, setCall, roLambda);
    }

    /// <summary>
    /// Registers a global ownership query filter for the resource entity, restricting access to resources owned by the current user.
    /// </summary>
    private static void SetResourceOwnershipFilter<TEntity, TId, TEntityResource>(ModelBuilder modelBuilder, FinanceDbContext context)
        where TEntity : Entity<TId>, new()
        where TEntityResource : EntityResource<TEntity, TId>
    {
        modelBuilder.Entity<TEntityResource>().HasQueryFilter(BuildResourceOwnershipFilter<TEntity, TId, TEntityResource>(context));
    }

    /// <summary>
    /// Builds a query filter expression for a resource entity, restricting access to resources owned by the current user.
    /// </summary>
    private static LambdaExpression BuildResourceOwnershipFilter<TEntity, TId, TEntityResource>(FinanceDbContext dbContext)
        where TEntity : Entity<TId>, new()
        where TEntityResource : EntityResource<TEntity, TId>
    {
        // r => ctx.Set<ResourceOwner>().Any(ro => ro.ResourceId == r.ResourceId && ro.User.Identities.Any(i => i.SourceId == ctx.CurrentUserId))
        var resourceParam = Expression.Parameter(typeof(TEntityResource), "r");
        var resourceIdProp = Expression.Property(resourceParam, nameof(EntityResource<TEntity, TId>.ResourceId));

        var resourceOwnerParam = Expression.Parameter(typeof(ResourceOwner), "ro");
        var roResourceIdProp = Expression.Property(resourceOwnerParam, nameof(ResourceOwner.ResourceId));
        var roUserProp = Expression.Property(resourceOwnerParam, nameof(ResourceOwner.User));
        var roUserIdentitiesProp = Expression.Property(roUserProp, nameof(User.Identities));

        var identParam = Expression.Parameter(typeof(Identity), "i");
        var sourceIdProp = Expression.Property(identParam, nameof(Identity.SourceId));
        var currentUserId = Expression.Property(Expression.Constant(dbContext), nameof(FinanceDbContext.CurrentUserId));
        var idEquals = Expression.Equal(sourceIdProp, currentUserId);
        var identLambda = Expression.Lambda(idEquals, identParam);

        var asQueryableMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethodDefinition)
            .MakeGenericMethod(typeof(Identity));
        var identitiesQueryable = Expression.Call(asQueryableMethod, roUserIdentitiesProp);
        var anyIdentityMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(Identity));
        var anyIdentityCall = Expression.Call(anyIdentityMethod, identitiesQueryable, identLambda);

        var roIdEquals = Expression.Equal(roResourceIdProp, resourceIdProp);
        var roBody = Expression.AndAlso(roIdEquals, anyIdentityCall);
        var roLambda = Expression.Lambda(roBody, resourceOwnerParam);

        var setCall = Expression.Call(
            Expression.Constant(dbContext),
            nameof(DbContext.Set),
            [typeof(ResourceOwner)]
        );
        var anyResourceOwnerMethod = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == nameof(Queryable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(ResourceOwner));
        var anyResourceOwnerCall = Expression.Call(anyResourceOwnerMethod, setCall, roLambda);

        return Expression.Lambda(anyResourceOwnerCall, resourceParam);
    }

    private static void SetCurrencyConversionOwnershipFilter(ModelBuilder modelBuilder, FinanceDbContext context)
    {
        modelBuilder.Entity<CurrencyConversion>().HasQueryFilter(BuildCurrencyConversionOwnershipFilter(context));
    }

    private static Expression<Func<CurrencyConversion, bool>> BuildCurrencyConversionOwnershipFilter(FinanceDbContext context)
    {
        return cc => cc.Movement != null && cc.Currency != null;
    }

    /// <summary>
    /// Sets ownership filters for credit card related entities through their navigation properties to CreditCard.
    /// This ensures users can only see statements, transactions, and payments for credit cards they own.
    /// </summary>
    private static void SetCreditCardRelatedOwnershipFilter(ModelBuilder modelBuilder, FinanceDbContext context)
    {
        modelBuilder.Entity<CreditCardStatement>().HasQueryFilter(
            s => context.Set<CreditCardResource>().Any(r =>
                r.ResourceSourceId == s.CreditCardId &&
                context.Set<ResourceOwner>().Any(ro =>
                    ro.ResourceId == r.ResourceId &&
                    ro.User.Identities.AsQueryable().Any(i => i.SourceId == context.CurrentUserId)
                )
            )
        );

        modelBuilder.Entity<CreditCardTransaction>().HasQueryFilter(
            t => context.Set<CreditCardResource>().Any(r =>
                r.ResourceSourceId == t.CreditCardId &&
                context.Set<ResourceOwner>().Any(ro =>
                    ro.ResourceId == r.ResourceId &&
                    ro.User.Identities.AsQueryable().Any(i => i.SourceId == context.CurrentUserId)
                )
            )
        );

        modelBuilder.Entity<CreditCardStatementTransaction>().HasQueryFilter(
            st => context.Set<CreditCardStatement>().Any(s =>
                s.Id == st.CreditCardStatementId &&
                context.Set<CreditCardResource>().Any(r =>
                    r.ResourceSourceId == s.CreditCardId &&
                    context.Set<ResourceOwner>().Any(ro =>
                        ro.ResourceId == r.ResourceId &&
                        ro.User.Identities.AsQueryable().Any(i => i.SourceId == context.CurrentUserId)
                    )
                )
            )
        );

        modelBuilder.Entity<CreditCardPayment>().HasQueryFilter(
            p => context.Set<CreditCardStatement>().Any(s =>
                s.Id == p.StatementId &&
                context.Set<CreditCardResource>().Any(r =>
                    r.ResourceSourceId == s.CreditCardId &&
                    context.Set<ResourceOwner>().Any(ro =>
                        ro.ResourceId == r.ResourceId &&
                        ro.User.Identities.AsQueryable().Any(i => i.SourceId == context.CurrentUserId)
                    )
                )
            )
        );

        modelBuilder.Entity<CreditCardStatementAdjustment>().HasQueryFilter(
            a => context.Set<CreditCardStatement>().Any(s =>
                s.Id == a.CreditCardStatementId &&
                context.Set<CreditCardResource>().Any(r =>
                    r.ResourceSourceId == s.CreditCardId &&
                    context.Set<ResourceOwner>().Any(ro =>
                        ro.ResourceId == r.ResourceId &&
                        ro.User.Identities.AsQueryable().Any(i => i.SourceId == context.CurrentUserId)
                    )
                )
            )
        );
    }
}
