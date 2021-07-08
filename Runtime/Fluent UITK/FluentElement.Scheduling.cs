using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace AV.UITK
{
    public partial class FluentElement<T> where T : VisualElement
    {
        public IVisualElementScheduledItem Schedule(Action<FluentElement<T>> action, long delay = -1)
        {
            var scheduled = schedule.Execute(() => action(this));
            
            if (delay != -1) 
                scheduled.StartingIn(delay);
            
            return scheduled;
        }
    }
}