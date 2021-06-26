using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public interface IUserElement {}

        public partial class FluentElement<T> where T : VisualElement
        {
            public virtual T x { get; internal set; }
            
            
            public string name { get => x.name; set => x.name = value; }
            
            public IStyle style => x.style;
            public IResolvedStyle resolvedStyle => x.resolvedStyle;
            public ITransitionAnimations animation => x.experimental.animation;
            public IVisualElementScheduler schedule => x.schedule;
            public VisualElementStyleSheetSet styleSheets => x.styleSheets;
            
            
            public FluentElement(T element) => x = element;

            public static implicit operator FluentElement<T>(T x) => new FluentElement<T>(x);
            public static implicit operator T(FluentElement<T> x) => x.x;
        }
        
        public partial class FluentElement : FluentElement<VisualElement>
        {
            public FluentElement(VisualElement element) : base(element) {}
            
            public static implicit operator FluentElement(VisualElement x) => new FluentElement(x);
            public static implicit operator VisualElement(FluentElement x) => x.x;
        }
    }
}