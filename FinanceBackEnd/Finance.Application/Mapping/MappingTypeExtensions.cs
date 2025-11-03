namespace Finance.Application.Mapping;

public static class MappingTypeExtensions
{
    public static bool HasSubclasses(this Type type)
        => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Any(t => t.BaseType == type);

    public static bool HasSubclasses(this object obj)
        => obj.GetType().HasSubclasses();
}
