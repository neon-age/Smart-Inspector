using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public class EditorElement : InspectorElement
        {
            public const string UserElementClass = "user-element";

            public override VisualElement x => element;
            
            public InspectorElement list { get; internal set; }
            public InspectorElement element { get; internal set; }
            public InspectorElement header { get; internal set; }
            public InspectorElement inspector { get; internal set; }
            public InspectorElement footer { get; internal set; }

            public int index { get; internal set; }

#if UNITY_EDITOR
            public Editor editor { get; internal set; }
            public ActiveEditorTracker tracker { get; internal set; }

            public Object target => editor.target;
            public Object[] targets => editor.targets;

            public bool isGo => target is GameObject;
            public bool isTransform => target is Transform;
            public bool isComponent => target is Component;
            public bool isMaterial => target is Material;
#endif

            public EditorElement(VisualElement element) : base(element)
            {
                this.element = element;
            }

            
            public StyleSheet FindStyleSheet(string name = null, string guid = null) => SmartInspector.FindStyleSheet(name, guid);
            public T FindAsset<T>(string name = null, string guid = null) where T : Object => SmartInspector.FindAsset<T>(name, guid);
            

            public bool IsTarget<T>(out T target) where T : Object
            {
                target = this.target as T;
                return target;
            }

            
            public InspectorElement NewImage(Texture background)
            {
                var image = new VisualElement { style = { backgroundImage = background as Texture2D }};
                return image;
            }
            
            public InspectorElement NewButton(string text, EventCallback<MouseUpEvent> onUp)
            {
                var button = new Button {text = text}.ForInspector();
                button.Register(onUp);
                return button;
            }
        }
    }
}