using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public interface IUserElement {}
        
        public partial class InspectorElement
        {
            public virtual VisualElement x { get; }
            
            
            public string name { get => x.name; set => x.name = value; }
            
            public IStyle style => x.style;
            public VisualElementStyleSheetSet styleSheets => x.styleSheets;
            
            
            public InspectorElement(VisualElement element) => x = element;

            public static implicit operator InspectorElement(VisualElement x) => new InspectorElement(x);
            public static implicit operator VisualElement(InspectorElement x) => x.x;
        }
    }
}