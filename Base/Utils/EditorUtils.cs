using System;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AV.Inspector
{
    internal static class EditorUtils
    {
        static Assembly EditorAssembly { get; } = typeof(Editor).Assembly;

        internal static class Presets
        {
            static Type menuType = EditorAssembly.GetType("UnityEditor.Presets.PresetContextMenu");
            
            static MethodInfo CreateAndShow = menuType.GetMethod("CreateAndShow", BindingFlags.NonPublic | BindingFlags.Static);

            public static void ShowMenu(params Object[] targets)
            {
                CreateAndShow.Invoke(null, new object[] { targets });
            }
        }
    }
}