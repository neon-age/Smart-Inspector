
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.UITK
{
    internal static class IStyleExtensions
    {
        const float n = float.NaN;
        
        static bool Nan(float value) => float.IsNaN(value);


        internal static void SetSize(this IStyle style, float width = n, float height = n)
        {
            if (!Nan(width)) style.width = width;
            if (!Nan(height)) style.height = height;
        }
        
        
        internal static void SetBorderColor(this IStyle style, Color color)
        {
            style.borderTopColor = color;
            style.borderLeftColor = color;
            style.borderRightColor = color;
            style.borderBottomColor = color;
        }

        internal static void SetBorderWidth(this IStyle style, float width)
        {
            SetBorderWidth(style, width, width, width, width);
        }

        internal static void SetBorderWidth(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.borderTopWidth = top;
            if (!Nan(left)) style.borderLeftWidth = left;
            if (!Nan(right)) style.borderRightWidth = right;
            if (!Nan(bottom)) style.borderBottomWidth = bottom;
        }

        internal static void SetBorderRadius(this IStyle style, float radius)
        {
            SetBorderRadius(style, radius, radius, radius, radius);
        }

        internal static void SetBorderRadius(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.borderTopLeftRadius = top;
            if (!Nan(left)) style.borderTopRightRadius = left;
            if (!Nan(right)) style.borderBottomLeftRadius = right;
            if (!Nan(bottom)) style.borderBottomRightRadius = bottom;
        }

        
        internal static void SetMargin(this IStyle style, float length)
        {
            SetMargin(style, length, length, length, length);
        }
        
        internal static void SetMargin(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.marginTop = top;
            if (!Nan(left)) style.marginLeft = left;
            if (!Nan(right)) style.marginRight = right;
            if (!Nan(bottom)) style.marginBottom = bottom;
        }

        
        internal static void SetPadding(this IStyle style, float length)
        {
            SetPadding(style, length, length, length, length);
        }

        internal static void SetPadding(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.paddingTop = top;
            if (!Nan(left)) style.paddingLeft = left;
            if (!Nan(right)) style.paddingRight = right;
            if (!Nan(bottom)) style.paddingBottom = bottom;
        }
        
        
        internal static void SetSlice(this IStyle style, int? top = null, int? left = null, int? right = null, int? bottom = null)
        {
            if (top.HasValue) style.unitySliceTop = top.Value;
            if (left.HasValue) style.unitySliceLeft = left.Value;
            if (right.HasValue) style.unitySliceRight = right.Value;
            if (bottom.HasValue) style.unitySliceBottom = bottom.Value;
        }
        
        
        internal static void SetPosition(this IStyle style, Position type, float top = n, float left = n, float right = n, float bottom = n)
        {
            style.position = type;
            SetPosition(style, top, left, right, bottom);
        }
        internal static void SetPosition(this IStyle style, float top = n, float left = n, float right = n, float bottom = n)
        {
            if (!Nan(top)) style.top = top;
            if (!Nan(left)) style.left = left;
            if (!Nan(right)) style.right = right;
            if (!Nan(bottom)) style.bottom = bottom;
        }
    }
}