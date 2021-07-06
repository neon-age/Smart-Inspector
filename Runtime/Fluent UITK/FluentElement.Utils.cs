using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AV.UITK
{
    public partial class FluentElement<T> where T : VisualElement
    {
        #region Assets
        public static StyleSheet FindStyleSheet(string name = null, string guid = null)
        {
            return FluentUITK.FindStyleSheet(name, guid);
        }
        
        public static IEnumerable<TType> FindAssets<TType>(string name = null) where TType : Object
        {
            return FluentUITK.FindAssets<TType>(name);
        }
        
        public static IEnumerable<Object> FindAssets(Type type, string name = null)
        {
            return FluentUITK.FindAssets<Object>(name);
        }
        
        public static TType FindAsset<TType>(string name = null, string guid = null) where TType : Object
        {
            return FluentUITK.FindAsset<TType>(name, guid);
        }
        #endregion
        
        #region Editor
        public static Texture2D GetObjectIcon(Object obj, Type type = null)
        {
            return FluentUITK.GetObjectIcon(obj, type);
        }

        public static Texture2D GetTypeIcon<TType>() => FluentUITK.GetTypeIcon<TType>();
        public static Texture2D GetTypeIcon(Type type) => FluentUITK.GetTypeIcon(type);
        public static Texture2D GetEditorIcon(string iconName) => FluentUITK.GetEditorIcon(iconName);
        #endregion
    }
}