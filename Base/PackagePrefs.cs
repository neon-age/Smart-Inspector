using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal class PackagePrefs : ScriptableObject
    {
        const string Guid = "93fb5defd30a56a499ebfe00af3102ef";
        
        #region Pref
        public class Pref<T> : ISerializationCallbackReceiver
        {
            public static Pref<T> Data;

            string[] keys = new string[0];
            T[] values = new T[0];
            Dictionary<string, T> prefs;

            public void OnAfterDeserialize()
            {
                prefs = new Dictionary<string, T>(keys.Length);
                for(int i = 0; i < keys.Length; i++)
                {
                    prefs.Add(keys[i], values[i]);
                }
                keys = null;
                values = null;
            }
            public void OnBeforeSerialize()
            {
                if (prefs == null)
                    prefs = new Dictionary<string, T>();
                keys = new string[prefs.Count];
                values = new T[prefs.Count];

                int i = 0;
                foreach (var pref in prefs)
                {
                    keys[i] = pref.Key;
                    values[i] = pref.Value;
                    i++;
                }
                prefs.Clear();
            }


            public T Get(string key, T defaultValue = default)
            {
                if (prefs == null)
                    OnAfterDeserialize();

                if (!prefs.TryGetValue(key, out var value))
                {
                    value = defaultValue;
                    prefs.Add(key, value);
                }
                return value;
            }

            public void Set(string key, T value)
            {
                if (prefs == null)
                    OnAfterDeserialize();

                if (prefs.ContainsKey(key))
                    prefs[key] = value;
                else
                    prefs.Add(key, value);
            }
        }
        #endregion

      
        static PackagePrefs Data => data ?? (data = AssetDatabase.LoadAssetAtPath<PackagePrefs>(AssetDatabase.GUIDToAssetPath(Guid)));
        static PackagePrefs data;
        
        static bool isInitialized;

        [Serializable]
        internal class SerializedStack<T>
        {
            [SerializeField] List<T> list = new List<T>();
            public int Count => list.Count;

            public void Clear() => list.Clear();
            public void Push(T values) => list.Add(values);
            public T Pop()
            {
                if (list.Count == 0)
                    return default;
                int lastIndex = list.Count - 1;
                var value = list[lastIndex];
                list.RemoveAt(lastIndex);
                return value;
            }
        }

        [Serializable]
        private class BoolPref : Pref<bool> { }
        [Serializable]
        private class IntPref : Pref<int> { }

        [Serializable]
        internal class IntsStack : SerializedStack<int[]> { }
        [Serializable]
        private class IntsStackPref : Pref<IntsStack> { }

        [SerializeField] string lastAbsolutePath;
        
        [SerializeField] BoolPref bools = new BoolPref();
        [SerializeField] IntPref ints = new IntPref();
        [SerializeField] IntsStackPref intsStack = new IntsStackPref();

        
        void Initialize()
        {
            if (PackageHasMoved())
            {
                bools = new BoolPref();
                ints = new IntPref();
                intsStack = new IntsStackPref();
                isInitialized = false;
            }
            
            if (isInitialized)
                return;

            BoolPref.Data = bools;
            IntPref.Data = ints;
            IntsStackPref.Data = intsStack;

            isInitialized = true;
        }

        private bool PackageHasMoved()
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(Guid);
            var dataPath = Application.dataPath;
            dataPath = dataPath.Remove(dataPath.Length - 6, 6);
            var absolutePath = dataPath + assetPath;

            if (lastAbsolutePath != absolutePath)
            {
                lastAbsolutePath = absolutePath;
                return true;
            }
            return false;
        }

        public static T Get<T>(string key, T defaultValue = default)
        {
            Data.Initialize();
            if (Pref<T>.Data == null)
            {
                Debug.LogError(typeof(T) + " is not supported.");
                return default;
            }
            return Pref<T>.Data.Get(key, defaultValue);
        }

        public static void Set<T>(string key, T value)
        {
            Data.Initialize();
            if (Pref<T>.Data == null)
            {
                Debug.LogError(typeof(T) + " is not supported.");
                return;
            }
            Pref<T>.Data.Set(key, value);
        }
    }
}
