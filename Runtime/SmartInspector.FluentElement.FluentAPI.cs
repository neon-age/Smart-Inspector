using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public partial class FluentElement<T> where T : VisualElement
        {
            public FluentElement<T> Clear()
            {
                x.Clear();
                return x;
            }
            
            public FluentElement<T> Add(VisualElement element)
            {
                element.AddToClassList(UserElementClass);
                x.Add(element);
                return x;
            }
            public FluentElement<T> Add(params VisualElement[] elements)
            {
                foreach (var element in elements)
                    Add(element);
                return x;
            }
            public FluentElement<T> Insert(int index, VisualElement element)
            {
                element.AddToClassList(UserElementClass);
                x.Insert(index, element);
                return x;
            }

            
            public FluentElement<T> Enabled(bool value)
            {
                x.SetEnabled(value);
                return x;
            }
            
            public FluentElement<T> Picking(bool enabled)
            {
                x.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
                return x;
            }


            public bool Has<TType>() where TType : VisualElement
            {
                return Get<TType>() != null;
            }
            public bool Has<TType>(string nameOrClass) where TType : VisualElement
            {
                return Get<TType>(nameOrClass) != null;
            }
            public bool Has(string nameOrClass)
            {
                return Get(nameOrClass) != null;
            }
            
            
            public TType Get<TType>() where TType : VisualElement
            {
                return x.Query<TType>();
            }
            public TType Get<TType>(string nameOrClass) where TType : VisualElement
            {
                if (nameOrClass.StartsWith(".")) return x.Query<TType>(className: nameOrClass.TrimStart('.'));
                if (nameOrClass.StartsWith("#")) return x.Query<TType>(name: nameOrClass.TrimStart('#'));
                return x.Query<TType>(nameOrClass);
            }
            public FluentElement Get(string nameOrClass)
            {
                if (nameOrClass.StartsWith(".")) return x.Query(className: nameOrClass.TrimStart('.')).First();
                if (nameOrClass.StartsWith("#")) return x.Query(name: nameOrClass.TrimStart('#')).First();
                
                return x.Query(nameOrClass).First();
            }
            
            
            public FluentElement<T> Name(string name)
            {
                x.name = NamePokaYoke(name);
                return x;
            }
            
            
            public bool HasClass(string className)
            {
                return x.ClassListContains(ClassPokaYoke(className));
            }
            public FluentElement<T> AddClass(params string[] classes)
            {
                foreach (var className in classes)
                    x.AddToClassList(ClassPokaYoke(className));
                
                return x;
            }
            public FluentElement<T> RemoveClass(params string[] classes)
            {
                foreach (var className in classes)
                    x.RemoveFromClassList(ClassPokaYoke(className));
                
                return x;
            }
            public FluentElement<T> EnableClass(string className, bool enable)
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