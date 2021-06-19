using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public class InspectorElement
        {
            const string UserElementClass = "user-element";
            
            const float n = float.NaN;
            
            public virtual VisualElement x { get; }
            
            public IStyle style => x.style;
            public string name { get => x.name; set => x.name = value; }

            public VisualElementStyleSheetSet styleSheets => x.styleSheets;
            
            public InspectorElement(VisualElement element) => x = element;

            public static implicit operator InspectorElement(VisualElement x) => new InspectorElement(x);
            public static implicit operator VisualElement(InspectorElement x) => x.x;
            
            
            public void SetPosition(Position type, float top = n, float left = n, float right = n, float bottom = n) => x.style.SetPosition(type, top, left, right, bottom);
            public void SetPosition(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetPosition(top, left, right, bottom);
            
            public void AlignSelf(Align align) => x.style.AlignSelf(align);
            
            public void SetSize(float width = n, float height = n) => x.style.SetSize(width, height);
            
            public void SetMargin(float margin) => x.style.SetMargin(margin);
            public void SetMargin(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetMargin(top, left, right, bottom);

            public void SetPadding(float padding) => x.style.SetPadding(padding);
            public void SetPadding(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetPadding(top, left, right, bottom);

            public void SetBorderColor(Color color) => x.style.SetBorderColor(color);
            
            public void SetBorderWidth(float width) => x.style.SetBorderWidth(width);
            public void SetBorderWidth(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetBorderWidth(top, left, right, bottom);
            
            public void SetBorderRadius(float radius) => x.style.SetBorderRadius(radius);
            public void SetBorderRadius(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetBorderRadius(top, left, right, bottom);
            
            
            
            public void Register<TEvent>(EventCallback<TEvent> evt) where TEvent : EventBase<TEvent>, new()
            {
                x.RegisterCallback(evt);
            }
            public void Unregister<TEvent>(EventCallback<TEvent> evt) where TEvent : EventBase<TEvent>, new()
            {
                x.UnregisterCallback(evt);
            }

            public void Add(VisualElement element)
            {
                element.AddToClassList(UserElementClass);
                x.Add(element);
            }
            public void Insert(int index, VisualElement element)
            {
                element.AddToClassList(UserElementClass);
                x.Insert(index, element);
            }

            
            public T Get<T>() where T : VisualElement
            {
                return x.Query<T>();
            }
            public T Get<T>(string nameOrClass) where T : VisualElement
            {
                if (nameOrClass.StartsWith(".")) return x.Query<T>(className: nameOrClass.TrimStart('.'));
                if (nameOrClass.StartsWith("#")) return x.Query<T>(name: nameOrClass.TrimStart('#'));
                return x.Query<T>(nameOrClass);
            }
            public VisualElement Get(string nameOrClass)
            {
                if (nameOrClass.StartsWith(".")) return x.Query(className: nameOrClass.TrimStart('.'));
                if (nameOrClass.StartsWith("#")) return x.Query(name: nameOrClass.TrimStart('#'));
                return x.Query(nameOrClass);
            }

            public InspectorElement SetName(string name)
            {
                x.name = NamePokaYoke(name);
                return x;
            }
            
            public bool HasClass(string className)
            {
                return x.ClassListContains(ClassPokaYoke(className));
            }
            public InspectorElement AddClass(params string[] classes)
            {
                foreach (var className in classes)
                    x.AddToClassList(ClassPokaYoke(className));
                
                return x;
            }
            public InspectorElement RemoveClass(params string[] classes)
            {
                foreach (var className in classes)
                    x.RemoveFromClassList(ClassPokaYoke(className));
                
                return x;
            }
            public InspectorElement EnableClass(string className, bool enable)
            {
                x.EnableInClassList(ClassPokaYoke(className), enable);
                return x;
            }

            string NamePokaYoke(string name)
            {
                if (name.StartsWith("#"))
                    return name.Remove(0, 1);
                return name;
            }
            string ClassPokaYoke(string className)
            {
                if (className.StartsWith("."))
                    return className.Remove(0, 1);
                return className;
            }
        }
    }
}