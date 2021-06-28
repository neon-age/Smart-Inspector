
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static class VisualElementExtensions
    {
        public static SmartInspector.FluentElement<TVisualElement> Fluent<TVisualElement>(this TVisualElement x) where TVisualElement : VisualElement
        {
            return x;
        }
        
        public static SmartInspector.FluentElement<VisualElement> Fluent(this IEventHandler x)
        {
            return x as VisualElement;
        }
    }
}