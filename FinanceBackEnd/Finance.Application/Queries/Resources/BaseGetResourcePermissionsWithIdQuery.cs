using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Persistence;

namespace Finance.Application.Queries.Resources;

public class BaseGetResourcePermissionsWithIdQuery<TSource, TId, TResourcePermissions> : BaseGetResourcePermissionsQuery<TSource, TId, TResourcePermissions>
    where TSource : Entity<TId>, new()
    where TResourcePermissions : ResourcePermissions<TSource, TId>
    where TId : struct
{
    public TId Id { get; }

    protected BaseGetResourcePermissionsWithIdQuery(TId id) : base()
    {
        Id = id;
    }
}

public class BaseGetResourcePermissionsWithIdQueryHandler<TSource, TId, TResourcePermissions>
    : BaseGetResourcePermissionsQueryHandler<BaseGetResourcePermissionsWithIdQuery<TSource, TId, TResourcePermissions>, TSource, TId, TResourcePermissions>
    where TSource : Entity<TId>, new()
    where TResourcePermissions : ResourcePermissions<TSource, TId>
    where TId : struct
{
    public BaseGetResourcePermissionsWithIdQueryHandler(FinanceDbContext dbContext) : base(dbContext)
    {
    }

    protected override IQueryable<TSource> SourceQuery(BaseGetResourcePermissionsWithIdQuery<TSource, TId, TResourcePermissions> request)
    {
        var id = request.Id;
        var query = DbContext.Set<TSource>();

        return typeof(TId) switch
        {
            var t when t == typeof(Guid) => query.Where(r => (Guid)(object)r.Id == (Guid)(object)id),
            var t when t == typeof(short) => query.Where(r => (short)(object)r.Id == (short)(object)id),
            var t when t == typeof(int) => query.Where(r => (int)(object)r.Id == (int)(object)id),
            var t when t == typeof(long) => query.Where(r => (long)(object)r.Id == (long)(object)id),
            var t when t == typeof(AppModuleTypeEnum) => query.Where(r => (AppModuleTypeEnum)(object)r.Id == (AppModuleTypeEnum)(object)id),
            var t when t == typeof(FrequencyEnum) => query.Where(r => (FrequencyEnum)(object)r.Id == (FrequencyEnum)(object)id),
            var t when t == typeof(IdentityProviderEnum) => query.Where(r => (IdentityProviderEnum)(object)r.Id == (IdentityProviderEnum)(object)id),
            var t when t == typeof(IOLInvestmentAssetTypeEnum) => query.Where(r => (IOLInvestmentAssetTypeEnum)(object)r.Id == (IOLInvestmentAssetTypeEnum)(object)id),
            var t when t == typeof(RoleEnum) => query.Where(r => (RoleEnum)(object)r.Id == (RoleEnum)(object)id),
            _ => throw new NotSupportedException($"Unsupported TId type for EF translation: {typeof(TId).Name}")
        };
    }
}
