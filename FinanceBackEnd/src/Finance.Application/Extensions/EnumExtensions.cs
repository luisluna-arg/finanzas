namespace Finance.Application.Extensions;

public static class EnumExtensions
{
    public static TEnum TryParse<TEnum>(this string value) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, true, out var result))
        {
            return result;
        }

        throw new ArgumentException($"Invalid enum value: {value}");
    }

    public static TEnum TryGet<TEnum>(this short value) where TEnum : struct, Enum
    {
        return ((long)value).TryGet<TEnum>();
    }

    public static TEnum TryGet<TEnum>(this int value) where TEnum : struct, Enum
    {
        return ((long)value).TryGet<TEnum>();
    }

    public static TEnum TryGet<TEnum>(this long value) where TEnum : struct, Enum
    {
        if (Enum.IsDefined(typeof(TEnum), value))
        {
            return (TEnum)(object)value;
        }

        throw new ArgumentException($"Invalid enum value: {value}");
    }
}
