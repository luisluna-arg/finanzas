namespace FinanceApi.Core.Extensions;

public static class DictionaryExtensions
{
    public static void Accumulate<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, new List<TValue>());
        dictionary[key].Add(value);
    }
}
