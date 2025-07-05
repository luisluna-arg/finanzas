namespace Finance.Application.Mapping;

public interface IMappingService
{
    TResult Map<TResult>(object source);
    ICollection<TResult> Map<TResult>(IEnumerable<object> source);
    object Map(object source, Type targetType);
    bool HasMapper<TSource, TTarget>();
    bool HasMapper(Type sourceType, Type targetType);
}