using System.Collections;

namespace Finance.Application.Mapping.Base;

public abstract class BaseMapper<TSource, TTarget> : IMapper<TSource, TTarget>
    where TTarget : class, new()
{
    protected IMappingService MappingService { get; private set; }

    protected BaseMapper(IMappingService mappingService)
    {
        MappingService = mappingService;
    }

    public bool IsMappingEnabled(Type sourceType, Type destinationType)
    {
        return sourceType == typeof(TSource) && destinationType == typeof(TTarget);
    }

    public object Map(object source)
    {
        if (source is TSource typedSource)
            return Map(typedSource)!;

        // If the source is a collection of TSource, map each item to TTarget
        if (source is IEnumerable<TSource> typedSourceCollection)
            return Map(typedSourceCollection).ToList();

        throw new InvalidCastException($"Expected type {typeof(TSource).Name} or IEnumerable<{typeof(TSource).Name}>, but got {source.GetType().Name}.");
    }

    public IEnumerable<object> Map(IEnumerable<object> source)
    {
        if (source is not IEnumerable<TSource> typedSource)
            throw new InvalidCastException($"Expected type {typeof(TSource).Name}, but got {source.GetType().Name}.");

        return Map(typedSource).Cast<object>()!;
    }

    public virtual TTarget Map(TSource source)
    {
        var target = new TTarget();
        MapCommonProperties(source, target);
        return target;
    }

    public virtual IEnumerable<TTarget> Map(IEnumerable<TSource> source)
        => source.Select(Map).ToList();

    protected virtual void MapCommonProperties(TSource source, TTarget target)
    {
        var sourceProperties = typeof(TSource).GetProperties();
        var targetProperties = typeof(TTarget).GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetProperties.FirstOrDefault(p =>
                p.Name == sourceProperty.Name &&
                p.CanWrite);

            if (targetProperty != null)
            {
                var value = sourceProperty.GetValue(source);

                // Always initialize collections, even if source is null
                if (IsCollectionType(targetProperty.PropertyType))
                {
                    var targetElementType = GetCollectionElementType(targetProperty.PropertyType);
                    if (value == null && targetElementType != null)
                    {
                        var emptyCollection = CreateCollectionOfType(targetProperty.PropertyType, targetElementType, new List<object>());
                        targetProperty.SetValue(target, emptyCollection);
                        continue;
                    }
                }

                // Handle collection mapping
                if (IsCollectionType(sourceProperty.PropertyType) && IsCollectionType(targetProperty.PropertyType))
                {
                    var sourceElementType = GetCollectionElementType(sourceProperty.PropertyType);
                    var targetElementType = GetCollectionElementType(targetProperty.PropertyType);
                    if (sourceElementType != null && targetElementType != null &&
                        MappingService.HasMapper(sourceElementType, targetElementType))
                    {
                        try
                        {
                            var sourceEnumerable = value == null ? new List<object>() : ((IEnumerable)value).Cast<object>();
                            var mappedItems = sourceEnumerable
                                .Select(item => MappingService.Map(item, targetElementType))
                                .ToList();
                            var targetCollection = CreateCollectionOfType(targetProperty.PropertyType, targetElementType, mappedItems);
                            targetProperty.SetValue(target, targetCollection);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error mapping collection property {sourceProperty.Name}: {ex.Message}");
                        }
                        continue;
                    }
                }

                if (value == null)
                {
                    targetProperty.SetValue(target, null);
                    continue;
                }

                // If property types match exactly, do direct assignment
                if (targetProperty.PropertyType == sourceProperty.PropertyType)
                {
                    targetProperty.SetValue(target, value);
                    continue;
                }

                // Handle complex type mapping
                if (!IsSimpleType(sourceProperty.PropertyType) && !IsSimpleType(targetProperty.PropertyType))
                {
                    if (MappingService.HasMapper(sourceProperty.PropertyType, targetProperty.PropertyType))
                    {
                        try
                        {
                            var mappedValue = MappingService.Map(value, targetProperty.PropertyType);
                            if (mappedValue != null)
                            {
                                targetProperty.SetValue(target, mappedValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error mapping property {sourceProperty.Name}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }

    private bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid)
            || IsNullableSimpleType(type);
    }

    private bool IsNullableSimpleType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return IsSimpleType(underlyingType);
            }
        }
        return false;
    }

    private bool IsCollectionType(Type type)
    {
        if (type == typeof(string)) return false;
        return typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType;
    }

    private Type? GetCollectionElementType(Type type)
    {
        if (type.IsArray)
            return type.GetElementType();
        if (type.IsGenericType)
            return type.GetGenericArguments().FirstOrDefault();
        return null;
    }

    private object? CreateCollectionOfType(Type collectionType, Type elementType, List<object> items)
    {
        if (collectionType.IsArray)
        {
            var array = Array.CreateInstance(elementType, items.Count);
            items.Select((item, i) => { array.SetValue(item, i); return item; }).ToList();
            return array;
        }
        if (typeof(IList).IsAssignableFrom(collectionType))
        {
            var constructedListType = typeof(List<>).MakeGenericType(elementType);

            var list = (IList)Activator.CreateInstance(constructedListType, items.ToArray())!;
            if (collectionType.IsAssignableFrom(constructedListType))
                return list;

            var ctor = collectionType.GetConstructor([constructedListType]);
            if (ctor != null)
                return ctor.Invoke([list]);
            if (collectionType.IsGenericType)
            {
                var genericDef = collectionType.GetGenericTypeDefinition();
                if (genericDef == typeof(ICollection<>) || genericDef == typeof(IEnumerable<>) || genericDef == typeof(IReadOnlyCollection<>))
                    return list;
            }
            return list;
        }
        if (collectionType.IsInterface && collectionType.IsGenericType)
        {
            var constructedListType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(constructedListType)!;

            foreach (var item in items)
            {
                list.Add(item);
            }

            return list;
        }
        return null;
    }
}
