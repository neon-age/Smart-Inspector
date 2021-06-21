using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public partial class EditorElement : InspectorElement
        {
            public InspectorElement NewHorizontalGroup(bool reverse = false)
            {
                return new HorizontalGroup(reverse);
            }
            public InspectorElement NewVerticalGroup(bool reverse = false)
            {
                return new VerticalGroup(reverse);
            }
            public InspectorElement NewIndentedGroup(int left = 15, int right = 1, bool useMargin = false, FlexDirection direction = FlexDirection.Column)
            {
                return new IndentedGroup(left, right, useMargin, direction);
            }
            
            
            public InspectorElement NewSpace(float width = float.NaN, float height = float.NaN)
            {
                return new Space(width, height);
            }
            public InspectorElement NewFlexibleSpace(float flexGrow = 1)
            {
                return new FlexibleSpace(flexGrow);
            }
            
            
            public InspectorElement NewText(string text)
            {
                return new Text(text);
            }
            public InspectorElement NewIcon(Texture background)
            {
                return new Image(background, 16).ForInspector().AddClass(IconClass);
            }
            public InspectorElement NewImage(Texture background, float maxSize = float.NaN)
            {
                return new Image(background, maxSize);
            }
            
            public InspectorElement NewButton(string text = default, Texture icon = default, 
                EventCallback<MouseDownEvent> onDown = default, EventCallback<MouseUpEvent> onUp = default)
            {
                var button = new Button().ForInspector();
                
                var container = new VisualElement();
                container.AddToClassList(ContentContainerClass);
                
                button.Add(container);
            
                if (icon != null)
                {
                    var iconField = NewIcon(icon);
                    container.Add(iconField);
                }
            
                if (text != null)
                {
                    var textField = new Text(text);
                    container.Add(textField);
                }
            
                if (onDown != null)
                    button.Register(onDown);
                
                if (onUp != null)
                    button.Register(onUp);
            
                return button;
            }
        }
    }
}