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
        if (source is not TSource typedSource)
            throw new InvalidCastException($"Expected type {typeof(TSource).Name}, but got {source.GetType().Name}.");

        return Map(typedSource)!;
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
                // Get the source property value
                var value = sourceProperty.GetValue(source);

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
                    // Check if there is a mapper for these types
                    if (MappingService.HasMapper(sourceProperty.PropertyType, targetProperty.PropertyType))
                    {
                        try
                        {
                            // Use the direct Map method with target type
                            var mappedValue = MappingService.Map(value, targetProperty.PropertyType);
                            if (mappedValue != null)
                            {
                                targetProperty.SetValue(target, mappedValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error for debugging purposes
                            System.Diagnostics.Debug.WriteLine($"Error mapping property {sourceProperty.Name}: {ex.Message}");
                            // If mapping fails, don't set the property
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
}