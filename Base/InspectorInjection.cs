
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    [InitializeOnLoad]
    internal static class InspectorInjection
    {
        const int UpdateRate = 10;
        
        static int editorFrames = UpdateRate;
        static EditorWindow lastFocusedWindow;
        
        static Dictionary<EditorWindow, SmartInspector> InjectedInspectors = new Dictionary<EditorWindow, SmartInspector>();
        
        
        static InspectorInjection()
        {
            EditorApplication.update += OnEditorUpdate;
            Selection.selectionChanged += FindInspectorWindowsAndInject;
        }

        static void OnEditorUpdate()
        {
            editorFrames = (editorFrames + 1) % UpdateRate; // only run every N frames
            if (editorFrames != 0) 
                return;
            
            var focusedWindow = EditorWindow.focusedWindow ?? EditorWindow.mouseOverWindow;

            if (focusedWindow == lastFocusedWindow)
                return;
            lastFocusedWindow = focusedWindow;

            FindInspectorWindowsAndInject();
        }
        
        static void FindInspectorWindowsAndInject()
        {
            //var windows = InspectorReflection.GetInspectors()
            //    .Where(x => x.GetType() == InspectorReflection.WindowType);
            var windows = Resources.FindObjectsOfTypeAll(PropertyEditorRef.type);
            
            foreach (EditorWindow window in windows)
            {
                if (!InjectedInspectors.ContainsKey(window))
                {
                    if (SmartInspector.TryInject(window, out var inspector))
                        InjectedInspectors.Add(window, inspector);
                }
            }
        }

        internal static void TryReinjectWindow(EditorWindow window)
        {
            InjectedInspectors.Remove(window);

            // Make sure to re-inject when switching between inspector modes, or other stuff
            FindInspectorWindowsAndInject();
            EditorApplication.delayCall += FindInspectorWindowsAndInject;
        }
    }
}