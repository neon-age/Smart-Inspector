using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AV.UITK
{
    public static partial class FluentUITK
    {
        static Dictionary<string, Texture2D> editorIconsCache = new Dictionary<string, Texture2D>();
        
        public static Texture2D GetObjectIcon(Object obj, Type type = null)
        {
            Texture2D icon = null; 
#if UNITY_EDITOR
            icon = EditorGUIUtility.ObjectContent(obj, type ?? obj.GetType()).image as Texture2D;
#endif
            return icon;
        }

        public static Texture2D GetTypeIcon<T>() => GetTypeIcon(typeof(T));
        public static Texture2D GetTypeIcon(Type type)
        {
            return GetObjectIcon(null, type);
        }
        
        public static Texture2D GetEditorIcon(string iconName)
        {
            if (!editorIconsCache.TryGetValue(iconName, out var icon))
            {
                #if UNITY_EDITOR
                icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;
                editorIconsCache.Add(iconName, icon);
                #endif
            }
            return icon;
        }
    }
}