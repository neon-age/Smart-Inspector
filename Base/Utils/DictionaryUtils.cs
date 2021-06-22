using System.Collections;
using System.Collections.Generic;

namespace AV.Inspector
{
    internal static class DictionaryUtils
    {
        public static void TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> lookup, TKey key)
        {
            if (lookup.ContainsKey(key))
                lookup.Remove(key);
        }
        public static void TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> lookup, TKey key, TValue value)
        {
            if (!lookup.ContainsKey(key))
                lookup.Add(key, value);
        }
        
        public static void AddOrAssign<TKey, TValue>(this Dictionary<TKey, TValue> lookup, TKey key, TValue value)
        {
            if (!lookup.ContainsKey(key))
                lookup.Add(key, value);
            else
                lookup[key] = value;
        }
    }
}