
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal static class InspectorInjection
    {
       
        

        /*
        internal static void InjectWindow(EditorWindow window, out SmartInspector inspector)
        {
            if (InjectedInspectors.TryGetValue(window, out inspector))
                return;
            
            if (SmartInspector.TryInject(window, out inspector))
            {
                //onInspectorRebuild += inspector.OnRebuildContent;
                InjectedInspectors.Add(window, inspector);
            }
        }

        internal static void UninjectWindow(EditorWindow window)
        {
            if (InjectedInspectors.TryGetValue(window, out var inspector))
            {
                //onInspectorRebuild -= inspector.OnRebuildContent;
                InjectedInspectors.Remove(window);
            }
        }
        
        internal static bool TryGetInspector(EditorWindow window, out SmartInspector inspector)
        {
            return InjectedInspectors.TryGetValue(window, out inspector);
        }*/
    }
}