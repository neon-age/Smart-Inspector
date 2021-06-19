
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal static class IStyleExtensions
    {
        const float n = float.NaN;
        
        static bool Nan(float value) => float.IsNaN(value);


        public static void AlignSelf(this IStyle style, Align align)
        {
            style.alignSelf = align;
        }
        
        
        public static void SetSize(this IStyle style, float width = n, float height = n)
        {
            if (!Nan(width)) style.width = width;
            if (!Nan(height)) style.height = height;
        }
        
        
        public static void SetBorderColor(this IStyle style, Color color)
        {
            style.borderTopColor = color;
            style.borderLeftColor = color;
            style.borderRightColor = color;
            style.borderBottomColor = color;
        }

        public static void SetBorderWidth(this IStyle style, float width)
        {
            SetBorderWidth(style, width, width, width, width);
        }

        public static void SetBorderWidth(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.borderTopWidth = top;
            if (!Nan(left)) style.borderLeftWidth = left;
            if (!Nan(right)) style.borderRightWidth = right;
            if (!Nan(bottom)) style.borderBottomWidth = bottom;
        }

        public static void SetBorderRadius(this IStyle style, float radius)
        {
            SetBorderRadius(style, radius, radius, radius, radius);
        }

        public static void SetBorderRadius(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.borderTopLeftRadius = top;
            if (!Nan(left)) style.borderTopRightRadius = left;
            if (!Nan(right)) style.borderBottomLeftRadius = right;
            if (!Nan(bottom)) style.borderBottomRightRadius = bottom;
        }

        
        public static void SetMargin(this IStyle style, float length)
        {
            SetMargin(style, length, length, length, length);
        }
        
        public static void SetMargin(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.marginTop = top;
            if (!Nan(left)) style.marginLeft = left;
            if (!Nan(right)) style.marginRight = right;
            if (!Nan(bottom)) style.marginBottom = bottom;
        }

        
        public static void SetPadding(this IStyle style, float length)
        {
            SetPadding(style, length, length, length, length);
        }

        public static void SetPadding(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.paddingTop = top;
            if (!Nan(left)) style.paddingLeft = left;
            if (!Nan(right)) style.paddingRight = right;
            if (!Nan(bottom)) style.paddingBottom = bottom;
        }
        
        
        public static void SetPosition(this IStyle style, Position type, float top = n, float left = n, float right = n, float bottom = n)
        {
            style.position = type;
            SetPosition(style, top, left, right, bottom);
        }
        public static void SetPosition(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.top = top;
            if (!Nan(left)) style.left = left;
            if (!Nan(right)) style.right = right;
            if (!Nan(bottom)) style.bottom = bottom;
        }
    }
}