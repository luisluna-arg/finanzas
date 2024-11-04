namespace Finance.Application.Helpers;

public static class EnumHelper
{
    public static List<T> GetEnumMembers<T>()
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("Type T must be an enum type.");
        }

        return new List<T>((T[])Enum.GetValues(typeof(T)));
    }

    public static TEnum Parse<TEnum>(string enumName)
        where TEnum : struct
        => Enum.TryParse(typeof(TEnum), enumName, out var enumValue) ? (TEnum)enumValue : default;
}
