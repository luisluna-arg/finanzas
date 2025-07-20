using System.Linq.Expressions;
using Finance.Domain.Models;
using Finance.Domain.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Finance.Persistance.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds global query filters to the model, such as user-ownership filters for entities like Fund.
    /// </summary>
    public static void AddQueryFilters(this ModelBuilder modelBuilder, FinanceDbContext context)
    {
        SetOWnerShipFilter<CreditCardMovement, Guid, CreditCardMovementResource>(modelBuilder, context);
        SetOWnerShipFilter<CreditCard, Guid, CreditCardResource>(modelBuilder, context);
        SetOWnerShipFilter<CreditCardStatement, Guid, CreditCardStatementResource>(modelBuilder, context);
        SetOWnerShipFilter<CurrencyExchangeRate, Guid, CurrencyExchangeRateResource>(modelBuilder, context);
        SetOWnerShipFilter<DebitOrigin, Guid, DebitOriginResource>(modelBuilder, context);
        SetOWnerShipFilter<Debit, Guid, DebitResource>(modelBuilder, context);
        SetOWnerShipFilter<Fund, Guid, FundResource>(modelBuilder, context);
        SetOWnerShipFilter<Income, Guid, IncomeResource>(modelBuilder, context);
        SetOWnerShipFilter<IOLInvestmentAsset, Guid, IOLInvestmentAssetResource>(modelBuilder, context);
        SetOWnerShipFilter<IOLInvestment, Guid, IOLInvestmentResource>(modelBuilder, context);
        SetOWnerShipFilter<Movement, Guid, MovementResource>(modelBuilder, context);
    }

    /// <summary>
    /// Registers a global ownership query filter for the given entity type, restricting access to entities owned by the current user.
    /// </summary>
    private static void SetOWnerShipFilter<TEntity, TId, TEntityResource>(ModelBuilder modelBuilder, FinanceDbContext context)
        where TEntity : Entity<TId>, new()
        where TEntityResource : EntityResource<TEntity, TId>
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(BuildFilter<TEntity, TId, TEntityResource>(context));
    }

    /// <summary>
    /// Builds a query filter expression for an entity, restricting access to entities joined to resources owned by the current user.
    /// </summary>
    private static LambdaExpression BuildFilter<TEntity, TId, TEntityResource>(FinanceDbContext dbContext)
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
}