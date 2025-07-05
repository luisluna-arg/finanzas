namespace Finance.Application.Mapping;

public class MappingService : IMappingService
{
    private readonly IEnumerable<IDtoMapper> _strategies;

    public MappingService()
    {
        var mapperType = typeof(IDtoMapper);

        _strategies = mapperType.Assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && mapperType.IsAssignableFrom(t))
            .Select(m =>
            {
                var constructor = m.GetConstructor([typeof(IMappingService)]);
                if (constructor == null)
                {
                    throw new InvalidOperationException($"Type {m.FullName} does not have a constructor accepting IDtoMapperManager.");
                }
                return constructor;
            })
            .Select(c => (IDtoMapper)c.Invoke([this]))
            .ToArray();
    }

    public TResult Map<TResult>(object source)
    {
        var mapper = _strategies
            .OrderBy(s => s.GetType().HasSubclasses())
            .FirstOrDefault(s => s.IsMappingEnabled(source.GetType(), typeof(TResult)));

        if (mapper != null)
        {
            return (TResult)mapper.Map(source);
        }

        throw new InvalidOperationException($"No mapper found for \"{source.GetType().Name}\" to \"{typeof(TResult).Name}\".");
    }

    public ICollection<TResult> Map<TResult>(IEnumerable<object> source)
        => source.Select(s => Map<TResult>(s)).ToList();
        
    public bool HasMapper<TSource, TTarget>()
    {
        return HasMapper(typeof(TSource), typeof(TTarget));
    }

    public bool HasMapper(Type sourceType, Type targetType)
    {
        return _strategies
            .OrderBy(s => s.GetType().HasSubclasses())
            .Any(s => s.IsMappingEnabled(sourceType, targetType));
    }
    
    public object Map(object source, Type targetType)
    {
        var sourceType = source.GetType();
        var mapper = _strategies
            .OrderBy(s => s.GetType().HasSubclasses())
            .FirstOrDefault(s => s.IsMappingEnabled(sourceType, targetType));

        if (mapper != null)
        {
            return mapper.Map(source);
        }

        throw new InvalidOperationException($"No mapper found for \"{sourceType.Name}\" to \"{targetType.Name}\".");
    }
}