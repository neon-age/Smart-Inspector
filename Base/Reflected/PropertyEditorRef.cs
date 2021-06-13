using System;
using System.Reflection;
using UnityEditor;
using static System.Linq.Expressions.Expression;
using Object = UnityEngine.Object;

namespace AV.Inspector
{
    internal static class PropertyEditorRef
    {
        internal static Type type { get; } = Type.GetType("UnityEditor.PropertyEditor, UnityEditor");
        
        static readonly PropertyInfo tracker = type.GetProperty("tracker");
        
        public static readonly Func<EditorWindow, Object> GetInspectedObject;
        
        static PropertyEditorRef()
        {
            var getInspectedObjectRef = type.GetMethod("GetInspectedObject", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var windowParam = Parameter(typeof(EditorWindow));
            GetInspectedObject = Lambda<Func<EditorWindow, Object>>(Call(Convert(windowParam, type), getInspectedObjectRef), windowParam).Compile();
        }

        public static ActiveEditorTracker GetTracker(EditorWindow propertyEditor)
        {
            return (ActiveEditorTracker)tracker.GetValue(propertyEditor);
        }
    }
}