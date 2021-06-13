
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        
        private static Type gameObjectInspectorType;
        
        private static FieldInfo hideInspectorField;
      
        
        private static PropertyInfo inspectorModeProperty;
        
        //private static MethodInfo getAllInspectorsMethod;
        private static Func<IEnumerable<EditorWindow>> getInspectors;
        private static MethodInfo onHeaderGUIMethod;
        
        
        static InspectorWindowRef()
        {
            inspectorModeProperty = type.GetProperty("inspectorMode");
            
            gameObjectInspectorType = Type.GetType("UnityEditor.GameObjectInspector, UnityEditor");
            
            var getAllInspectorsMethod = type.GetMethod("GetInspectors", BindingFlags.NonPublic | BindingFlags.Static);
            ExpressionUtils.CreateDelegate(getAllInspectorsMethod, out getInspectors);

            hideInspectorField = typeof(Editor).GetField("hideInspector", BindingFlags.NonPublic | BindingFlags.Instance);

            onHeaderGUIMethod = gameObjectInspectorType.GetMethod("OnHeaderGUI", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        internal static IEnumerable<EditorWindow> GetInspectors()
        {
            var allInspectors = getInspectors.Invoke();
            
            return allInspectors;
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