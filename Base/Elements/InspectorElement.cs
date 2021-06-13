using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorElement : VisualElement
    {
        protected internal SmartInspector Inspector { get; internal set; } = SmartInspector.LastInjected;
    }
}