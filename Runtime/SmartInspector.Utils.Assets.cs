using System;
using System.Collections.Generic;
using System.Linq;
using AV.UITK;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        #region Assets
        public static StyleSheet FindStyleSheet(string name = null, string guid = null)
        {
            return FluentUITK.FindStyleSheet(name, guid);
        }
        
        public static IEnumerable<T> FindAssets<T>(string name = null) where T : Object
        {
            return FluentUITK.FindAssets<T>(name);
        }
        
        public static IEnumerable<Object> FindAssets(Type type, string name = null)
        {
            return FluentUITK.FindAssets<Object>(name);
        }
        
        public static T FindAsset<T>(string name = null, string guid = null) where T : Object
        {
            return FluentUITK.FindAsset<T>(name, guid);
        }
        #endregion
    }
}