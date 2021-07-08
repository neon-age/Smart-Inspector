

using System;
using UnityEngine.UIElements;

namespace AV.UITK
{
    public class CallHandle
    {
        public FluentElement x { get; internal set; }
        public Delegate callback { get; internal set; }
        public EventBase evt { get; internal set; }
        public int totalCalls { get; internal set; }

        internal long eventTypeId;
        internal TrickleDown trickle;
        
        public void Use()
        {
            evt.StopPropagation();
        }
        public void Unregister()
        {
            x.Unregister(this);
        }
    }
}