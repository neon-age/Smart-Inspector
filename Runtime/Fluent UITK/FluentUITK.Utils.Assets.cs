using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AV.UITK
{
    public static partial class FluentUITK
    {
        static readonly string[] emptyGUIDs = new string[0];
        
        static class LoadedAssets<T> where T : Object
        {
            public static readonly Dictionary<string, T> ByGUID = new Dictionary<string, T>();
        }
        
        
        public static StyleSheet FindStyleSheet(string name = null, string guid = null)
        {
            return FindAsset<StyleSheet>(name, guid);
        }
        
        
        static IEnumerable<string> FindAssetsGUIDs(Type type, string name = null)
        {
            string[] matches = emptyGUIDs;

            var typeFilter = $"t:{type.Name}";
                
            #if UNITY_EDITOR
            if (name != null)
                matches = AssetDatabase.FindAssets($"{typeFilter} {name}");
            else
                matches = AssetDatabase.FindAssets($"{typeFilter}");
            #endif

            foreach (var guid in matches)
                yield return guid;
        }
        
        
        public static IEnumerable<T> FindAssets<T>(string name = null) where T : Object
        {
            var matches = FindAssetsGUIDs(typeof(T), name);
            
            foreach (var guid in matches)
                yield return LoadAssetByGUID<T>(guid);
        }
        
        public static IEnumerable<Object> FindAssets(Type type, string name = null)
        {
            var matches = FindAssetsGUIDs(type, name);
            
            foreach (var guid in matches)
                yield return LoadAssetByGUID(type, guid);
        }

        
        public static T FindAsset<T>(string name = null, string guid = null) where T : Object
        {
            if (guid != null)
                return LoadAssetByGUID<T>(guid);

            if (name != null)
                return (T)FindAssets(typeof(T), name).FirstOrDefault();
            
            // By type
            return FindAssets(typeof(T)).FirstOrDefault() as T;
        }
        
        
        
        static T LoadAssetByGUID<T>(string guid) where T : Object
        {
            if (LoadedAssets<T>.ByGUID.TryGetValue(guid, out var asset)) 
                return asset;

            return LoadAssetByGUID(typeof(T), guid) as T;
        }
        
        static Object LoadAssetByGUID(Type type, string guid)
        {
            Object asset = null;
            
            #if UNITY_EDITOR
            var path = AssetDatabase.GUIDToAssetPath(guid);
            asset = AssetDatabase.LoadAssetAtPath(path, type);
            #endif
            
            return asset;
        }
    }
}