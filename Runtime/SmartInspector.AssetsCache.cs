using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        static class LoadedAssets<T> where T : Object
        {
            public static readonly Dictionary<string, T> ByName = new Dictionary<string, T>();
            public static readonly Dictionary<string, T> ByGUID = new Dictionary<string, T>();
        }

        public static StyleSheet FindStyleSheet(string name = null, string guid = null)
        {
            return FindAsset<StyleSheet>(name, guid);
        }

        public static T FindAsset<T>(string name = null, string guid = null) where T : Object
        {
            var asset = default(T);

            if (name != null)
            {
                if (LoadedAssets<T>.ByName.TryGetValue(name, out asset))
                    return asset;

                var matches = AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}");

                if (matches.Length > 0)
                    guid = matches[0];
            }

            if (guid != null)
            {
                if (!LoadedAssets<T>.ByGUID.TryGetValue(guid, out asset))
                {
                    asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));

                    if (name != null)
                        LoadedAssets<T>.ByName.Add(name, asset);

                    LoadedAssets<T>.ByGUID.Add(guid, asset);
                }
            }

            return asset;
        }
    }
}