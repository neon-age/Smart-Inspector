
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal static class EditorElementRef
    {
        public static readonly Type type = typeof(PropertyField).Assembly.GetType("UnityEditor.UIElements.EditorElement");
        
        static PropertyInfo editor = type.GetProperty("editor");
        static FieldInfo m_EditorIndex = type.GetField("m_EditorIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        
        static EditorElementRef()
        {
        }

        public static Editor GetEditor(VisualElement editorElement)
        {
            return (Editor)editor.GetValue(editorElement);
        }

        public static int GetEditorIndex(VisualElement editorElement)
        {
            return (int)m_EditorIndex.GetValue(editorElement);
        }
    }
}