using UnityEngine.UIElements;
using VE = UnityEngine.UIElements.VisualElement;

namespace AV.UITK
{
    public static class IFluentExtensions
    {
        // TODO: Move all FluentElement utils into IFluent extension methods
        /*
        public static T Style<T>(this IFluent<T> e, params Styles[] styles) where T : VE
        {
            foreach (var style in styles)
                FluentUITK.DefineStyle(e.x, style);
            return e.x;
        }
        
        public static float Shrink<T>(this IFluent<T> e) where T : VE => e.x.resolvedStyle.flexShrink;
        public static T Shrink<T>(this IFluent<T> e, float flexShrink) where T : VE
        {
            e.x.style.flexShrink = flexShrink; return e.x;
        }
        */
    }
}