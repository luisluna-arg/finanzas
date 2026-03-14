namespace Finance.Application.Mapping;

public class MappingService : IMappingService
{
    private readonly Dictionary<(Type, Type), IDtoMapper> _mapperLookup;

    public MappingService()
    {
        var mapperType = typeof(IDtoMapper);
        var iMapperType = typeof(IMapper<,>);

        // Pre-sort by HasSubclasses() so leaf (more-specific) mappers are registered first.
        // TryAdd below ensures the first mapper wins when multiple mappers share the same type pair.
        var mappers = mapperType.Assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && mapperType.IsAssignableFrom(t))
            .OrderBy(t => t.HasSubclasses())
            .Select(m =>
            {
                var constructor = m.GetConstructor([typeof(IMappingService)]);
                if (constructor == null)
                    throw new InvalidOperationException($"Type {m.FullName} does not have a constructor accepting IDtoMapperManager.");
                return constructor;
            })
            .Select(c => (IDtoMapper)c.Invoke([this]))
            .ToList();

        // Build O(1) lookup keyed by (TSource, TTarget) extracted from IMapper<TSource, TTarget>.
        _mapperLookup = new Dictionary<(Type, Type), IDtoMapper>(mappers.Count);
        foreach (var mapper in mappers)
        {
            var iface = mapper.GetType()
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == iMapperType);
            if (iface == null) continue;
            var args = iface.GetGenericArguments();
            _mapperLookup.TryAdd((args[0], args[1]), mapper);
        }
    }

    private IDtoMapper? FindMapper(Type sourceType, Type targetType)
    {
        if (_mapperLookup.TryGetValue((sourceType, targetType), out var mapper))
            return mapper;

        // Unwrap collection element types and retry (e.g. ICollection<Entity> → ICollection<Dto>).
        var sourceElement = GetCollectionElementType(sourceType);
        var targetElement = GetCollectionElementType(targetType);
        if (sourceElement != null && targetElement != null &&
            _mapperLookup.TryGetValue((sourceElement, targetElement), out mapper))
            return mapper;

        return null;
    }

    private static Type? GetCollectionElementType(Type type)
    {
        if (type.IsArray) return type.GetElementType();
        if (type.IsGenericType) return type.GetGenericArguments().FirstOrDefault();
        return null;
    }

    public TResult Map<TResult>(object source)
    {
        var mapper = FindMapper(source.GetType(), typeof(TResult));
        if (mapper != null)
            return (TResult)mapper.Map(source);

        throw new InvalidOperationException($"No mapper found for \"{source.GetType().FullName}\" to \"{typeof(TResult).FullName}\".");
    }

    public ICollection<TResult> Map<TResult>(IEnumerable<object> source)
        => source.Select(s => Map<TResult>(s)).ToList();

    public bool HasMapper<TSource, TTarget>()
        => HasMapper(typeof(TSource), typeof(TTarget));

    public bool HasMapper(Type sourceType, Type targetType)
        => FindMapper(sourceType, targetType) != null;

    public object Map(object source, Type targetType)
    {
        var mapper = FindMapper(source.GetType(), targetType);
        if (mapper != null)
            return mapper.Map(source);

        throw new InvalidOperationException($"No mapper found for \"{source.GetType().Name}\" to \"{targetType.Name}\".");
    }
}
