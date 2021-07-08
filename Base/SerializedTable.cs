using System;
using System.Collections.Generic;
using UnityEngine;

namespace AV.Inspector
{
    [Serializable]
    internal class SerializedTable<TKey, TValue> : ISerializationCallbackReceiver
    {
        [Serializable]
        public struct Pair
        {
            public TKey key;
            public TValue value;
        }
        
        [SerializeField] List<Pair> list = new List<Pair>();
        
        public readonly Dictionary<TKey, TValue> table = new Dictionary<TKey, TValue>();
        
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            list.Clear();
    
            foreach (var kvp in table)
            {
                list.Add(new Pair { key = kvp.Key, value = kvp.Value });
            }
        }
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            table.Clear();

            foreach (var pair in list)
                table.Add(pair.key, pair.value);
        }

        public TValue Get(TKey key, TValue @default = default)
        {
            if (!TryGet(key, out var value))
                Set(key, @default);
            return value;
        }
        public bool TryGet(TKey key, out TValue value)
        {
            return table.TryGetValue(key, out value);
        }
        public void Set(TKey key, TValue value)
        {
            if (table.ContainsKey(key))
                table[key] = value;
            else
                table.Add(key, value);
        }
    }
}