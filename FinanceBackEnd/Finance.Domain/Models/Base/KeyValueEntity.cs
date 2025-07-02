using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class KeyValueEntity<TEnum>() : AuditedEntity<TEnum>()
    where TEnum : struct, Enum
{
    public static readonly string DefaultName = "Default";

    public required string Name { get; set; }

    public static T Default<T>(string? name = null) where T : KeyValueEntity<TEnum>
    {
        // Use reflection to create an instance and set the Name property
        var instance = (T)Activator.CreateInstance(typeof(T))!;
        instance.Name = name ?? DefaultName;
        return instance;
    }

    public override string ToString() => Name;
}