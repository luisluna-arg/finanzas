namespace Finance.Domain.Models.Base;

public class KeyValueEntity<TEnum, TSelf> : AuditedEntity<TEnum>
    where TEnum : struct, Enum
    where TSelf : KeyValueEntity<TEnum, TSelf>, new()
{
    public static readonly string DefaultName = "Default";

    public string Name { get; set; } = string.Empty;

    public KeyValueEntity() : base()
    {
    }

    public static TSelf Default(string? name = null)
    {
        var instance = new TSelf();
        instance.Name = name ?? DefaultName;
        return instance;
    }

    public static TSelf Create(TEnum @enum)
    {
        var instance = new TSelf();
        instance.Id = @enum;
        instance.Name = @enum.ToString();
        return instance;
    }

    public static TSelf Create(string @enumName)
    {
        if (!Enum.TryParse<TEnum>(@enumName, out var @enum))
            throw new ArgumentException($"Invalid enum name: {@enumName}");

        var instance = new TSelf();
        instance.Id = @enum;
        instance.Name = @enum.ToString();
        return instance;
    }

    public static TSelf Create(short enumValue)
    {
        if (!Enum.IsDefined(typeof(TEnum), enumValue))
            throw new ArgumentException($"Invalid enum value: {enumValue}");

        var instance = new TSelf();
        instance.Id = (TEnum)(object)enumValue;
        instance.Name = enumValue.ToString();
        return instance;
    }

    public override string ToString() => Name;
}
