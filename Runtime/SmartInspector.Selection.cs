
using System.Linq;
using UnityEngine;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public static EditorSelection Selection { get; } = new EditorSelection();
        
        public class EditorSelection
        {
            public void SetGameObjects(params GameObject[] gameObjects)
            {
                #if UNITY_EDITOR
                UnityEditor.Selection.objects = gameObjects;
                #endif
            }
            public void SetGameObjects(params Component[] components)
            {
                #if UNITY_EDITOR
                UnityEditor.Selection.objects = components.Select(x => (Object)x.gameObject).ToArray();
                #endif
            }
            public void SetObjects(params Object[] objects)
            {
                #if UNITY_EDITOR
                UnityEditor.Selection.objects = objects;
                #endif
            }
        }
    }
}