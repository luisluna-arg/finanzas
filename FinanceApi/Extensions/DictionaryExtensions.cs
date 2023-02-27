namespace FinanceApi.Extensions;

internal static class DictionaryExtensions
{
    public static void Accumulate<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, new List<TValue>());
        }
        dictionary[key].Add(value);
    }
}
