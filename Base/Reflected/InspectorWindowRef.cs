
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
using UnityEditor;
using static System.Linq.Expressions.Expression;
using Object = UnityEngine.Object;

namespace AV.Inspector
{
    internal enum InspectorMode
    {
        Default = 0,
        Debug = 1,
        DebugInternal = 2
    }
    
    [InitializeOnLoad]
    internal static class InspectorWindowRef
    {
        internal static Type type { get; } = Type.GetType("UnityEditor.InspectorWindow, UnityEditor");
        
        static PropertyInfo inspectorModeProperty = AccessTools.Property(type, "inspectorMode");
        
        
        static InspectorWindowRef()
        {
        }

        
        public static void SwitchDebugMode(EditorWindow inspector)
        {
            var mode = GetInspectorMode(inspector);
            SetInspectorMode(inspector, mode == InspectorMode.Default ? InspectorMode.Debug : InspectorMode.Default);
        }
        
        public static InspectorMode GetInspectorMode(EditorWindow inspector)
        {
            return (InspectorMode)inspectorModeProperty.GetValue(inspector);
        }
        
        public static void SetInspectorMode(EditorWindow inspector, InspectorMode mode)
        {
            inspectorModeProperty.SetValue(inspector, (int)mode);
        }
    }
}