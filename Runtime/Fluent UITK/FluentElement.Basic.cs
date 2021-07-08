using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace AV.UITK
{
    public partial class FluentElement<T> where T : VisualElement
    {
        #region Sugar
        public struct ElementsList
        {
            public List<FluentElement> list;

            public void Do(Action<FluentElement<VisualElement>> action)
            {
                foreach (var e in list)
                    action.Invoke(e);
            }
        }
        
        public FluentElement<T> This(Action<T> action)
        {
            action.Invoke(x);
            return x;
        }
        
        public ElementsList For(params VisualElement[] elements)
        {
            return new ElementsList
            {
                list = elements.Select(e => new FluentElement(e)).ToList()
            };
        }

        public void Out(out FluentElement<T> element)
        {
            element = this;
        }
        #endregion


        public FluentElement<T> Clear()
        {
            x.Clear();
            return x;
        }
        
        public FluentElement<T> Add(VisualElement element)
        {
            Insert(x.childCount, element);
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
            FluentUITK.AddCoreStyles(element);
            
            x.Insert(index, element);
            onAddChild?.Invoke(element);
            
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