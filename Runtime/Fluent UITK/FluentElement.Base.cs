using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace AV.UITK
{
    public partial class FluentElement<T> where T : VisualElement
    {
        public virtual T x { get; }
        internal Action<VisualElement> onAddChild; // TODO: Expose this?

        public string name
        {
            get => x.name;
            set => x.name = value;
        }

        public FluentElement parent => x.parent;

        public IStyle style => x.style;
        public IResolvedStyle resolvedStyle => x.resolvedStyle;
        public ITransitionAnimations animation => x.experimental.animation;
        public IVisualElementScheduler schedule => x.schedule;
        public VisualElementStyleSheetSet styleSheets => x.styleSheets;


        public FluentElement(T x) => this.x = x;
        public FluentElement<VisualElement> this[int index] => x[index]?.Fluent();

        public static implicit operator T(FluentElement<T> x) => x?.x;
        public static implicit operator FluentElement<T>(T x) => new FluentElement<T>(x);
        public static implicit operator FluentElement<T>(UQueryBuilder<T> x) => new FluentElement<T>(x);

        public override string ToString() => x?.ToString();
    }

    public partial class FluentElement : FluentElement<VisualElement>
    {
        public FluentElement(VisualElement element) : base(element) {}

        public static implicit operator FluentElement(VisualElement x) => new FluentElement(x);
        public static implicit operator VisualElement(FluentElement x) => x.x;
        public static implicit operator FluentElement(UQueryBuilder<VisualElement> x) => new FluentElement(x);
    }
}