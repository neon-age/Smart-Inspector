using UnityEngine;
using UnityEngine.UIElements;
using UIE = UnityEngine.UIElements;
using static AV.UITK.FluentUITK;

namespace AV.UITK
{
    public partial class FluentElement<T> where T : VisualElement
    {
        const float n = float.NaN;
        static bool Nan(float value) => float.IsNaN(value);


        public FluentElement<T> Style(Styles styles)
        {
            FluentUITK.DefineStyle(x, styles);
            return x;
        }
        public FluentElement<T> Style(params Styles[] styles)
        {
            foreach (var style in styles)
                FluentUITK.DefineStyle(x, style);
            return x;
        }

        public DisplayStyle Display() => x.resolvedStyle.display;
        public FluentElement<T> Display(bool flex)
        {
            x.style.display = flex ? DisplayStyle.Flex : DisplayStyle.None; return x;
        }
        
        public Visibility Visible() => x.resolvedStyle.visibility;
        public FluentElement<T> Visible(bool visible)
        {
            x.style.visibility = visible ? Visibility.Visible : Visibility.Hidden; return x;
        }

        
        public FlexDirection Direction() => x.resolvedStyle.flexDirection;
        public FluentElement<T> Direction(FlexDirection direction)
        {
            x.style.flexDirection = direction; return x;
        }

        public float Grow() => x.resolvedStyle.flexGrow;
        public FluentElement<T> Grow(float flexGrow)
        {
            x.style.flexGrow = flexGrow; return x;
        }
        
        public float Shrink() => x.resolvedStyle.flexShrink;
        public FluentElement<T> Shrink(float flexShrink)
        {
            x.style.flexShrink = flexShrink; return x;
        }
        

        public SidesTopLeft TopLeft()
        {
            var r = x.resolvedStyle;
            return new SidesTopLeft(r.top, r.left);
        }
        public FluentElement<T> TopLeft(float top, float left)
        {
            x.style.SetPosition(top: top, left: top); return x;
        }
        
        public FluentElement<T> StretchToParent()
        {
            Position(UIE.Position.Absolute, 0, 0, 0, 0); return x;
        }
        
        public Positioning Position()
        {
            var r = x.resolvedStyle;
            return new Positioning(r.position, new Sides(r.top, r.left, r.right, r.bottom));
        }
        public FluentElement<T> Position(Position type, float top = n, float left = n, float right = n, float bottom = n)
        {
            x.style.SetPosition(type, top, left, right, bottom); return x;
        }
        public FluentElement<T> Position(float top = n, float left = n, float right = n, float bottom = n)
        {
            x.style.SetPosition(top, left, right, bottom); return x;
        }
        
        public Sides Margin()
        {
            var r = x.resolvedStyle;
            return new Sides(r.marginTop, r.marginLeft, r.marginRight, r.marginBottom);
        }
        public FluentElement<T> Margin(float margin)
        {
            x.style.SetMargin(margin); return x;
        }
        public FluentElement<T> Margin(float top = n, float left = n, float right = n, float bottom = n)
        {
            x.style.SetMargin(top, left, right, bottom); return x;
        }

        public Sides Padding()
        {
            var r = x.resolvedStyle;
            return new Sides(r.paddingTop, r.paddingLeft, r.paddingRight, r.paddingBottom);
        }
        public FluentElement<T> Padding(float padding)
        {
            x.style.SetPadding(padding); return x;
        }
        public FluentElement<T> Padding(float top = n, float left = n, float right = n, float bottom = n)
        {
            x.style.SetPadding(top, left, right, bottom); return x;
        }
        
        public FluentElement<T> Slice(int? top = default, int? left = default, int? right = default, int? bottom = default)
        {
            x.style.SetSlice(top, left, right, bottom); return x;
        }



        public Align AlignSelf() => x.resolvedStyle.alignContent;
        public FluentElement<T> AlignSelf(Align align) { x.style.alignSelf = align; return x; }
        
        public Align AlignItems() => x.resolvedStyle.alignItems;
        public FluentElement<T> AlignItems(Align align) { x.style.alignItems = align; return x; }

        public Align AlignContent() => x.resolvedStyle.alignContent;
        public FluentElement<T> AlignContent(Align align) { x.style.alignContent = align; return x; }
        
        public Justify JustifyContent() => x.resolvedStyle.justifyContent;
        public FluentElement<T> JustifyContent(Justify justify) { x.style.justifyContent = justify; return x; }

        
        public Vector2 Size()
        {
            var r = x.resolvedStyle;
            return new Vector2(r.width, r.height);
        }
        public FluentElement<T> Size(float width = n, float height = n)
        {
            x.style.SetSize(width, height); return x;
        }

        public Vector2 MinSize()
        {
            var r = x.resolvedStyle;
            return new Vector2(r.maxWidth.value, r.maxHeight.value);
        }
        public FluentElement<T> MinSize(float minWidth = n, float minHeight = n)
        {
            x.style.minWidth = minWidth;
            x.style.minHeight = minHeight;
            return x;
        }

        public Vector2 MaxSize()
        {
            var r = x.resolvedStyle;
            return new Vector2(r.maxWidth.value, r.maxHeight.value);
        }
        public FluentElement<T> MaxSize(float maxWidth = n, float maxHeight = n)
        {
            x.style.maxWidth = maxWidth;
            x.style.maxHeight = maxHeight;
            return x;
        }

        
        public float Opacity() => x.resolvedStyle.opacity;
        public FluentElement<T> Opacity(float opacity)
        {
            x.style.opacity = opacity; return x;
        }
        
        public Color Color() => x.resolvedStyle.backgroundColor;
        public FluentElement<T> Color(Color color)
        {
            x.style.backgroundColor = color; return x;
        }

        public Color TextColor() => x.resolvedStyle.color;
        public FluentElement<T> TextColor(Color color)
        {
            x.style.color = color; return x;
        }

        #if UNITY_2020_1_OR_NEWER
        public Texture Image() => x.resolvedStyle.backgroundImage.texture;
        #endif
        public FluentElement<T> Image(Texture texture)
        {
            x.style.backgroundImage = texture as Texture2D; return x;
        }
        
        public Color ImageTint() => x.resolvedStyle.unityBackgroundImageTintColor;
        public FluentElement<T> ImageTint(Color color)
        {
            x.style.unityBackgroundImageTintColor = color; return x;
        }
        
        
        public float FontSize() => x.resolvedStyle.fontSize;
        public FluentElement<T> FontSize(float size)
        {
            x.style.fontSize = size; return x;
        }
        
        public Font Font() => x.resolvedStyle.unityFont;
        public FluentElement<T> Font(Font font)
        {
            x.style.unityFont = font; return x;
        }
        
        public FontStyle FontStyle() => x.resolvedStyle.unityFontStyleAndWeight;
        public FluentElement<T> FontStyle(FontStyle style)
        {
            x.style.unityFontStyleAndWeight = style; return x;
        }


        public TextAnchor TextAlign() => x.resolvedStyle.unityTextAlign;
        public FluentElement<T> TextAlign(TextAnchor align)
        {
            x.style.unityTextAlign = align; return x;
        }

        #if UNITY_2020_1_OR_NEWER
        public TextOverflow TextOverflow() => x.resolvedStyle.textOverflow;
        public FluentElement<T> TextOverflow(TextOverflow overflow)
        {
            x.style.textOverflow = overflow; return x;
        }
        #endif
        public FluentElement<T> Overflow(Overflow overflow)
        {
            x.style.overflow = overflow; return x;
        }
        public FluentElement<T> Overflow(bool visible)
        {
            x.style.overflow = visible ? UIE.Overflow.Visible : UIE.Overflow.Hidden; return x;
        }


        public SidesColor BorderColor()
        {
            var r = x.resolvedStyle;
            return new SidesColor(r.borderTopColor, r.borderLeftColor, r.borderRightColor, r.borderBottomColor);
        }
        public FluentElement<T> BorderColor(Color color)
        {
            x.style.SetBorderColor(color); return x;
        }
        public FluentElement<T> BorderColor(SidesColor colors)
        {
            return BorderColor(colors.top, colors.left, colors.right, colors.bottom);
        }
        public FluentElement<T> BorderColor(Color? top = default, Color? left = default, Color? right = default, Color? bottom = default)
        {
            if (top.HasValue) x.style.borderTopColor = top.Value;
            if (left.HasValue) x.style.borderLeftColor = left.Value;
            if (right.HasValue) x.style.borderRightColor = right.Value;
            if (bottom.HasValue) x.style.borderBottomColor = bottom.Value;
            return x;
        }


        public Sides BorderWidth()
        {
            var r = x.resolvedStyle;
            return new Sides(r.borderTopWidth, r.borderLeftWidth, r.borderRightWidth, r.borderBottomWidth);
        } 
        public FluentElement<T> BorderWidth(float width)
        {
            x.style.SetBorderWidth(width); return x;
        }
        public FluentElement<T> BorderWidth(Sides sides)
        {
            return BorderWidth(sides);
        }
        public FluentElement<T> BorderWidth(float top = n, float left = n, float right = n, float bottom = n)
        {
            x.style.SetBorderWidth(top, left, right, bottom); return x;
        }

        
        public Corners BorderRadius()
        {
            var r = x.resolvedStyle;
            return new Corners(r.borderTopLeftRadius, r.borderLeftWidth, r.borderRightWidth, r.borderBottomWidth);
        } 
        public FluentElement<T> BorderRadius(float radius)
        {
            x.style.SetBorderRadius(radius); return x;
        }
        public FluentElement<T> BorderRadius(Corners corners)
        {
            return BorderRadius(corners.topLeft, corners.topRight, corners.bottomLeft, corners.bottomRight);
        }
        public FluentElement<T> BorderRadius(
            float topLeft = n, float topRight = n, float bottomLeft = n, float bottomRight = n,
            float top = n, float left = n, float right = n, float bottom = n)
        {
            var c = new Corners(topLeft, topRight, bottomLeft, bottomRight);
            c = c.ToSides(top, left, right, bottom);
            
            x.style.SetBorderRadius(c.topLeft, c.topRight, c.bottomLeft, c.bottomRight); return x;
        }
    }
}