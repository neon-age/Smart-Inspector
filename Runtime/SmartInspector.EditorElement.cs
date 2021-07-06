#if UNITY_EDITOR
using UnityEditor;
#endif
using AV.UITK;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        /// Note: the class itself is inherited from <see cref="FluentElement"/>
        public partial class EditorElement : FluentElement
        {
            public override VisualElement x => element;
            
            public FluentElement list { get; internal set; }
            public FluentElement element { get; internal set; }
            
            public FluentElement<IMGUIContainer> header { get; internal set; }
            public FluentElement<VisualElement> inspector { get; internal set; }
            internal FluentElement<IMGUIContainer> footer { get; set; }

            public int index { get; internal set; }
            public bool isExpanded => expandedState == 1;
            internal int expandedState;
            
            public bool isGo => target is GameObject;
            public bool isTransform => target is Transform;
            public bool isComponent => target is Component;
            public bool isMaterial => target is Material;

            public EditorSelection Selection => SmartInspector.Selection;

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

            public EditorElement(VisualElement x) : base(x)
            {
                this.element = x;
            }
            
            public bool IsTarget<T>(out T target) where T : Object
            {
                target = this.target as T;
                return target;
            }
            public bool IsTarget<T>() where T : Object
            {
                return this.target is T;
            }
            

            public void RebuildInspector()
            {
                #if UNITY_EDITOR
                tracker.ForceRebuild();
                #endif
            }
        }
    }
}