using System;
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
            public InspectorElement NewIndentedGroup(int left = 15, int right = 1, int top = 0, int bottom = 0,
                bool useMargin = false, FlexDirection direction = FlexDirection.Column)
            {
                return new IndentedGroup(left, right, top, bottom, useMargin, direction);
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
                return new Text(text).ForInspector();
            }
            
            public InspectorElement NewIcon(Texture background)
            {
                return new Icon(background);
            }
            
            public InspectorElement NewImage(Texture background, float maxSize = float.NaN)
            {
                return new Image(background, maxSize).ForInspector();
            }
            
            
            public InspectorElement NewToolbarButton(string text = default, Texture icon = default, 
                EventCallback<MouseUpEvent> onUp = default, EventCallback<MouseDownEvent> onDown = default)
            {
                return SetupButton(new ToolbarButton(text, icon), onUp, onDown);
            }
            
            public InspectorElement NewButton(string text = default, Texture icon = default, 
                EventCallback<MouseUpEvent> onUp = default, EventCallback<MouseDownEvent> onDown = default)
            {
                return SetupButton(new Button(text, icon), onUp, onDown);
            }

            static Button SetupButton(Button button, EventCallback<MouseUpEvent> onUp = default, EventCallback<MouseDownEvent> onDown = default)
            {
                var isMouseOver = false;

                if (onUp != null)
                {
                    button.RegisterCallback<MouseEnterEvent>(evt => isMouseOver = true);
                    button.RegisterCallback<MouseLeaveEvent>(evt => isMouseOver = false);
                }

                if (onDown != null)
                    button.RegisterCallback<MouseDownEvent>(onDown);
                
                if (onUp != null)
                    button.RegisterCallback<MouseUpEvent>(evt =>
                    {
                        if (isMouseOver)
                            onUp.Invoke(evt);
                    });
                
                return button;
            }
        }
    }
}