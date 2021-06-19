
using UnityEngine;
using EditorSelection = UnityEditor.Selection;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public static SelectionWrap Selection { get; } = new SelectionWrap();
        
        public class SelectionWrap
        {
            public void Set(params Object[] objects)
            {
                #if UNITY_EDITOR
                UnityEditor.Selection.objects = objects;
                #endif
            }
        }
    }
}