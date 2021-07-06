
using UnityEngine.UIElements;

namespace AV.UITK
{
    public static class VisualElementExtensions
    {
        public static FluentElement<TVisualElement> Fluent<TVisualElement>(this TVisualElement x) where TVisualElement : VisualElement
        {
            return x;
        }
        
        public static FluentElement<VisualElement> Fluent(this IEventHandler x)
        {
            return x as VisualElement;
        }
    }
}