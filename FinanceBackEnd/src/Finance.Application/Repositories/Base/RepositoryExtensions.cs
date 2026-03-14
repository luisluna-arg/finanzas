using System.Linq.Expressions;
using Finance.Application.Repositories.Base;

public static class RepositoryExtensions
{
    public static IQueryable<TEntity> FilterBy<TEntity>(this IQueryable<TEntity> query, string searchCriteria, ExpressionOperator expressionOperator, object searchValue)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, searchCriteria);
        var value = Expression.Constant(searchValue);

        Expression operation;
        switch (expressionOperator)
        {
            case ExpressionOperator.GreaterThan:
                operation = Expression.GreaterThan(property, value);
                break;
            case ExpressionOperator.GreaterThanOrEqual:
                operation = Expression.GreaterThanOrEqual(property, value);
                break;
            case ExpressionOperator.LessThan:
                operation = Expression.LessThan(property, value);
                break;
            case ExpressionOperator.LessThanOrEqual:
                operation = Expression.LessThanOrEqual(property, value);
                break;
            default:
                operation = Expression.Equal(property, value);
                break;
        }

        return query.Where(Expression.Lambda<Func<TEntity, bool>>(operation, parameter));
    }
}
