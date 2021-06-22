using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        /// Note: the class itself is also wrapped around <see cref="InspectorElement"/>
        public partial class EditorElement : InspectorElement
        {
            public override VisualElement x => element;
            
            public InspectorElement list { get; internal set; }
            public InspectorElement element { get; internal set; }
            public InspectorElement header { get; internal set; }
            public InspectorElement inspector { get; internal set; }
            internal InspectorElement footer { get; set; }

            public int index { get; internal set; }
            public bool isExpanded => expandedState == 1;
            internal int expandedState;
            
            public bool isGo => target is GameObject;
            public bool isTransform => target is Transform;
            public bool isComponent => target is Component;
            public bool isMaterial => target is Material;

#if !UNITY_EDITOR
            public Object target => null;
            public Object[] targets => null;
#else
            internal object smartInspector;
            
            public Editor editor { get; internal set; }
            public EditorWindow window { get; internal set; }
            public ActiveEditorTracker tracker { get; internal set; }

            public Object target => editor.target;
            public Object[] targets => editor.targets;
#endif

            public EditorElement(VisualElement element) : base(element)
            {
                this.element = element;
            }
            
            public bool IsTarget<T>(out T target) where T : Object
            {
                target = this.target as T;
                return target;
            }
            

            public void RefreshInspector()
            {
                #if UNITY_EDITOR
                tracker.ForceRebuild();
                #endif
            }
        }
    }
}