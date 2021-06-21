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
                base.text = text;
            }
        }
        
        public class Image : VisualElement, IUserElement
        {
            public Image(Texture texture, float maxSize = float.NaN)
            {
                style.backgroundImage = texture as Texture2D;

                if (!float.IsNaN(maxSize))
                {
                    style.maxWidth = maxSize;
                    style.maxHeight = maxSize;
                }
            }
        }
        
        
        public class Group : VisualElement, IUserElement
        {
            public Group(FlexDirection direction)
            {
                style.flexDirection = direction;
            }
        }
        public class IndentedGroup : Group, IUserElement
        {
            public IndentedGroup(int left = 15, int right = 1, bool useMargin = false, FlexDirection direction = FlexDirection.Column) : base(direction)
            {
                if (useMargin)
                {
                    style.marginLeft = left;
                    style.marginRight = right;
                }
                else
                {
                    style.paddingLeft = left;
                    style.paddingRight = right;
                }
            }
        }
        public class HorizontalGroup : Group, IUserElement
        {
            public HorizontalGroup(bool reverse = false) : base(reverse ? FlexDirection.Row : FlexDirection.RowReverse) {}
        }
        public class VerticalGroup : Group, IUserElement
        {
            public VerticalGroup(bool reverse = false) : base(reverse ? FlexDirection.Column : FlexDirection.ColumnReverse) {}
        }
        
        
        public class Space : VisualElement, IUserElement
        {
            public Space(float width = float.NaN, float height = float.NaN)
            {
                if (!float.IsNaN(width)) style.width = width;
                if (!float.IsNaN(height)) style.height = height;
            }
        }

        public class FlexibleSpace : VisualElement, IUserElement
        {
            public FlexibleSpace(float flexGrow = 1)
            {
                style.flexGrow = flexGrow;
            }
        }
    }
}