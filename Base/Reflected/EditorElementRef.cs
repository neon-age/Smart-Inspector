
using System;
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal static class EditorElementRef
    {
        public static readonly Type type = typeof(PropertyField).Assembly.GetType("UnityEditor.UIElements.EditorElement");
        
        static PropertyInfo editor = AccessTools.Property(type, "editor");
        static FieldInfo m_EditorIndex = AccessTools.Field(type, "m_EditorIndex");
        static FieldInfo m_WasVisible = AccessTools.Field(type, "m_WasVisible");
        static PropertyInfo m_InspectorElement = AccessTools.Property(type, "m_InspectorElement");

        static MethodInfo reinit = AccessTools.Method(type, "Reinit");
        
        static Func<VisualElement, Editor> getEditor;
        static Func<VisualElement, int> getEditorIndex;
        static Func<VisualElement, bool> getWasVisible;
        static Func<VisualElement, InspectorElement> getInspectorElement;
        
        
        static EditorElementRef()
        {
            var elementParam = Expression.Parameter(typeof(VisualElement));
            var elementConvert = Expression.Convert(elementParam, type);

            getEditor = Expression.Lambda<Func<VisualElement, Editor>>(Expression.Property(elementConvert, editor), elementParam).Compile();
            getEditorIndex = Expression.Lambda<Func<VisualElement, int>>(Expression.Field(elementConvert, m_EditorIndex), elementParam).Compile();
            getWasVisible = Expression.Lambda<Func<VisualElement, bool>>(Expression.Field(elementConvert, m_WasVisible), elementParam).Compile();
            getInspectorElement = Expression.Lambda<Func<VisualElement, InspectorElement>>(Expression.Property(elementConvert, m_InspectorElement), elementParam).Compile();
        }


        public static void Reinit(VisualElement editorElement)
        {
            var editorIndex = GetEditorIndex(editorElement);
            reinit.Invoke(editorElement, new object[] { editorIndex });
        }

        public static Editor GetEditor(VisualElement editorElement)
        {
            return getEditor(editorElement);
        }

        public static int GetEditorIndex(VisualElement editorElement)
        {
            return getEditorIndex(editorElement);
        }
        
        public static bool WasVisible(VisualElement editorElement)
        {
            return getWasVisible(editorElement);
        }
        
        public static InspectorElement GetInspectorElement(VisualElement editorElement)
        {
            return getInspectorElement(editorElement);
        }
    }
}