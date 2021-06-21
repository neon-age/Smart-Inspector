using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public partial class InspectorElement
        {
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