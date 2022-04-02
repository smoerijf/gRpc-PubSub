namespace Client.Shared
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }
            return dict[key] = factory(key);
        }
    }
}
