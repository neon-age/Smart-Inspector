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
        }
    }
}