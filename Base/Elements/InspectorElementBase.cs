using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorElementBase : VisualElement
    {
        protected internal SmartInspector Inspector { get; internal set; } = SmartInspector.LastActive;
    }
}