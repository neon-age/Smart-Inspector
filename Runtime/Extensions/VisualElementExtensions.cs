
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static class VisualElementExtensions
    {
        public static SmartInspector.InspectorElement ForInspector(this VisualElement x)
        {
            return x;
        }
    }
}