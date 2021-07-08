using System;
using System.Reflection;
using HarmonyLib;
using UnityEditor;
using UnityEngine;
using static System.Linq.Expressions.Expression;
using Object = UnityEngine.Object;

namespace AV.Inspector
{
    internal static class PropertyEditorRef
    {
        internal static Type type { get; } = 
        #if UNITY_2020_1_OR_NEWER
            typeof(Editor).Assembly.GetType("UnityEditor.PropertyEditor");
        #else
            typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        #endif
        
        static readonly PropertyInfo tracker = AccessTools.Property(type, "tracker");
        
        public static readonly Func<EditorWindow, Object> GetInspectedObject;
        
        static PropertyEditorRef()
        {
            var getInspectedObjectRef = type.GetMethod("GetInspectedObject", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var windowParam = Parameter(typeof(EditorWindow));
            GetInspectedObject = Lambda<Func<EditorWindow, Object>>(Call(Convert(windowParam, type), getInspectedObjectRef), windowParam).Compile();
        }

        public static ActiveEditorTracker GetTracker(object propertyEditor)
        {
            return (ActiveEditorTracker)tracker.GetValue(propertyEditor);
        }

        public static void RebuildAllInspectors()
        {
            foreach (var window in Resources.FindObjectsOfTypeAll(type))
            {
                var tracker = GetTracker(window);
                tracker.ForceRebuild();
            }
        }
    }
}