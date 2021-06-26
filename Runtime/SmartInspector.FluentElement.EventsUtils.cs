
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public partial class FluentElement<T>
        {
            public delegate void Event<in TEvent>(TEvent evt, FluentElement<T> e);

            static class EventStorage<TEvent>
            {
                public static Dictionary<Event<TEvent>, EventCallback<TEvent>> Registered = new Dictionary<Event<TEvent>, EventCallback<TEvent>>();
            }
            

            public FluentElement<T> This(Action<FluentElement<T>> action)
            {
                action.Invoke(x);
                return x;
            }
            
            
            public FluentElement<T> Register<TEvent>(Event<TEvent> callback) where TEvent : EventBase<TEvent>, new()
            {
                if (EventStorage<TEvent>.Registered.ContainsKey(callback))
                    return x;

                var handler = (EventCallback<TEvent>)(call => callback(call, this));
                
                x.RegisterCallback(handler);
                
                EventStorage<TEvent>.Registered.Add(callback, handler);
                
                return x;
            }

            public FluentElement<T> RegisterClick(EventCallback<MouseUpEvent> onUp)
            {
                var isMouseOver = false;
                
                x.RegisterCallback<MouseEnterEvent>(evt => isMouseOver = true);
                x.RegisterCallback<MouseLeaveEvent>(evt => isMouseOver = false);
                
                x.RegisterCallback<MouseUpEvent>(evt =>
                {
                    if (isMouseOver)
                        onUp.Invoke(evt);
                });
                return x;
            }
            
            public FluentElement<T> RegisterOneShot<TEvent>(EventCallback<TEvent> callback) where TEvent : EventBase<TEvent>, new()
            {
                EventCallback<TEvent> handler = null;
                
                handler = evt => 
                {
                    callback.Invoke(evt);
                    x.UnregisterCallback(handler);
                };
                
                x.RegisterCallback(handler);
                return x;
            }
            
            public FluentElement<T> Register<TEvent>(EventCallback<TEvent> callback) where TEvent : EventBase<TEvent>, new()
            {
                x.RegisterCallback(callback);
                return x;
            }
            
            
            public FluentElement<T> Unregister<TEvent>(Event<TEvent> callback) where TEvent : EventBase<TEvent>, new()
            {
                if (EventStorage<TEvent>.Registered.TryGetValue(callback, out var evt))
                    x.UnregisterCallback(evt);
                return x;
            }
            
            public FluentElement<T> Unregister<TEvent>(EventCallback<TEvent> callback) where TEvent : EventBase<TEvent>, new()
            {
                x.UnregisterCallback(callback);
                return x;
            }
        }
    }
}