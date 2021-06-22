using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public class Text : TextElement, IUserElement
        {
            public Text(string text)
            {
                AddToClassList(TextClass);
                base.text = text;
            }
        }
        
        public class Image : VisualElement, IUserElement
        {
            public Image(Texture texture = default, float maxSize = float.NaN)
            {
                AddToClassList(ImageClass);
                style.backgroundImage = texture as Texture2D;

                if (!float.IsNaN(maxSize))
                {
                    style.maxWidth = maxSize;
                    style.maxHeight = maxSize;
                }
            }
        }
        public class Icon : Image, IUserElement
        {
            public Icon(Texture texture = default) : base(texture, 16) => AddToClassList(IconClass);
        }

        
        public class Button : UnityEngine.UIElements.Button, IUserElement
        {
            public override VisualElement contentContainer { get; } = new Content();

            public Button(string text = default, Texture icon = default)
            {
                AddToClassList(ButtonClass);
                RemoveFromClassList("unity-text-element");
                
                hierarchy.Add(contentContainer);
                
                if (icon != null)
                {
                    var iconField = new Icon(icon);
                    Add(iconField);
                }
            
                if (text != null)
                {
                    var textField = new Text(text);
                    Add(textField);
                }
            }
        }
        public class ToolbarButton : Button
        {
            public ToolbarButton(string text = default, Texture icon = default) : base(text, icon)
            {
                AddToClassList(ToolbarButtonClass);
                AddToClassList("unity-toolbar-button");
                //RemoveFromClassList("unity-button");
            }
        }
        
        
        // We use this, users should use Group
        internal class Content : VisualElement
        {
            public Content()
            {
                pickingMode = PickingMode.Ignore;
                AddToClassList(ContentContainerClass);
            }
        }
        
        public class Group : VisualElement, IUserElement
        {
            public Group(FlexDirection direction)
            {
                pickingMode = PickingMode.Ignore;
                style.flexDirection = direction;
            }
        }
        public class IndentedGroup : Group, IUserElement
        {
            public IndentedGroup(int left = 15, int right = 1, int top = 0, int bottom = 0, 
                bool useMargin = false, FlexDirection direction = FlexDirection.Column) : base(direction)
            {
                if (useMargin)
                {
                    style.marginLeft = left;
                    style.marginRight = right;
                    style.marginTop = top;
                    style.marginBottom = bottom;
                }
                else
                {
                    style.paddingLeft = left;
                    style.paddingRight = right;
                    style.paddingTop = top;
                    style.paddingBottom = bottom;
                }
            }
        }
        public class HorizontalGroup : Group, IUserElement
        {
            public HorizontalGroup(bool reverse = false) : base(reverse ? FlexDirection.RowReverse : FlexDirection.Row) {}
        }
        public class VerticalGroup : Group, IUserElement
        {
            public VerticalGroup(bool reverse = false) : base(reverse ? FlexDirection.ColumnReverse : FlexDirection.Column) {}
        }
        
        
        public class Space : VisualElement, IUserElement
        {
            public Space(float width = float.NaN, float height = float.NaN)
            {
                pickingMode = PickingMode.Ignore;
                
                if (!float.IsNaN(width)) style.width = width;
                if (!float.IsNaN(height)) style.height = height;
            }
        }

        public class FlexibleSpace : VisualElement, IUserElement
        {
            public FlexibleSpace(float flexGrow = 1)
            {
                pickingMode = PickingMode.Ignore;
                style.flexGrow = flexGrow;
            }
        }
    }
}