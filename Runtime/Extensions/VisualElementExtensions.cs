
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static class VisualElementExtensions
    {
        public static SmartInspector.FluentElement<TVisualElement> Fluent<TVisualElement>(this TVisualElement x) where TVisualElement : VisualElement
        {
            return x;
        }
    }
}