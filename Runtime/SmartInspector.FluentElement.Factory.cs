
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public partial class FluentElement<T>
        {
            public FluentElement NewElement()
            {
                return new VisualElement();
            }
            public FluentElement<TElement> New<TElement>() where TElement : VisualElement, new()
            {
                return new TElement();
            }


            public FluentElement<Foldout> NewFoldout(string text)
            {
                return new Foldout { text = text };
            }
            
            public FluentElement<HorizontalGroup> NewHorizontalGroup(bool reverse = false)
            {
                return new HorizontalGroup(reverse);
            }
            public FluentElement<VerticalGroup> NewVerticalGroup(bool reverse = false)
            {
                return new VerticalGroup(reverse);
            }
            public FluentElement<Group> NewGroup()
            {
                return new Group();
            }
            public FluentElement<Group> NewIndentedGroup(int left = 15, int right = 1, int top = 0, int bottom = 0,
                bool useMargin = false, FlexDirection direction = UnityEngine.UIElements.FlexDirection.Column)
            {
                var group = new Group(direction).Fluent();
                
                if (useMargin)
                    group.Margin(top, left, right, bottom);
                else
                    group.Padding(top, left, right, bottom);
                
                return group;
            }
            
            
            public FluentElement<Space> NewSpace(float width = float.NaN, float height = float.NaN)
            {
                return new Space(width, height);
            }
            public FluentElement<FlexibleSpace> NewFlexibleSpace(float flexGrow = 1)
            {
                return new FlexibleSpace(flexGrow);
            }
            
            
            public FluentElement<Text> NewText(string text)
            {
                return new Text(text);
            }
            
            public FluentElement<Icon> NewIcon(Texture background, float maxSize = 16)
            {
                return new Icon(background, maxSize);
            }
            
            
            public FluentElement<ToolbarButton> NewToolbarButton(string text = default, Texture icon = default, 
                EventCallback<MouseUpEvent> onUp = default, EventCallback<MouseDownEvent> onDown = default)
            {
                var button = new ToolbarButton(text, icon);
                SetupClickEvents(button, onUp, onDown);
                return button;
            }
            
            public FluentElement<Button> NewButton(string text = default, Texture icon = default, 
                EventCallback<MouseUpEvent> onUp = default, EventCallback<MouseDownEvent> onDown = default)
            {
                var button = new Button(text, icon);
                SetupClickEvents(button, onUp, onDown);
                return button;
            }
            
            
            static void SetupClickEvents(VisualElement x, EventCallback<MouseUpEvent> onUp = default, EventCallback<MouseDownEvent> onDown = default)
            {
                var isMouseOver = false;
            
                if (onUp != null)
                {
                    x.RegisterCallback<MouseEnterEvent>(evt => isMouseOver = true);
                    x.RegisterCallback<MouseLeaveEvent>(evt => isMouseOver = false);
                }
            
                if (onDown != null)
                    x.RegisterCallback<MouseDownEvent>(onDown);
                
                if (onUp != null)
                    x.RegisterCallback<MouseUpEvent>(evt =>
                    {
                        if (isMouseOver)
                            onUp.Invoke(evt);
                    });
            }
        }
    }
}