using FinanceApi.Domain;
using FinanceApi.Domain.Models;

public static class InvestmentAssetIOLTypeEnumTableConverter
{
    public static void ConvertEnumToTable<TEnum, TEntity>(FinanceDbContext context, Func<string, TEntity> entityFactory)
        where TEnum : Enum
        where TEntity : class
    {
        var enumType = typeof(TEnum);
        var enumValues = Enum.GetValues(enumType);

        foreach (var value in enumValues)
        {
            context.Set<TEntity>().Add(entityFactory(value.ToString()!));
        }

        context.SaveChanges();
    }

    public static void ConvertTableToEnum<TEnum, TEntity>(FinanceDbContext context, Dictionary<TEnum, TEntity> entityMap)
        where TEnum : Enum
        where TEntity : class, IEnumEntity, new()
    {
        foreach (var enumCasting in Enum.GetValues(typeof(TEnum)))
        {
            var enumValue = (TEnum)enumCasting;
            if (entityMap.ContainsKey(enumValue)) continue;

            var entity = new TEntity()
            {
                Id = Convert.ToInt32(enumValue),
                Name = enumValue.ToString()
            };

            entityMap[enumValue] = entity;
            context.Set<TEntity>().Add(entity);
        }

        context.SaveChanges();
    }
}
