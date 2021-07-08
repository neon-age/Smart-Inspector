
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.UITK
{
    public partial class FluentElement<T>
    {
        public delegate void Call<in TEvent>(TEvent evt, CallHandle c) where TEvent : EventBase<TEvent>, new();

        static class EventStorage<TEvent>  where TEvent : EventBase<TEvent>, new()
        {
            public static Dictionary<Call<TEvent>, CallHandle> Handlers = new Dictionary<Call<TEvent>, CallHandle>();
        }

        /// <summary> Returns all callbacks registered to this element. </summary>
        public IEnumerable<FluentUITK.EventFunctor> GetCallbacks()
        {
            return FluentUITK.GetCallbacks(x);
        }

        
        // TODO: OnBind
        // TODO: OnAnyChange
        
        public FluentElement<T> OnChange<TValue>(Call<ChangeEvent<TValue>> evt) { Register(evt); return x; }
        public FluentElement<T> OnChange<TValue>(EventCallback<ChangeEvent<TValue>> evt) { Register(evt); return x; }
        public FluentElement<T> OnChange<TValue>(Action evt)
        {
            Register<ChangeEvent<TValue>>(_ => evt()); return x;
        }

        public FluentElement<T> OnAttach(Call<AttachToPanelEvent> evt) { Register(evt); return x; }
        public FluentElement<T> OnAttach(EventCallback<AttachToPanelEvent> evt) { Register(evt); return x; }
        public FluentElement<T> OnAttach(Action evt)
        {
            Register<AttachToPanelEvent>(_ => evt()); return x;
        }

        public FluentElement<T> OnLayoutUpdate(Call<GeometryChangedEvent> evt) { Register(evt); return x; }
        public FluentElement<T> OnLayoutUpdate(EventCallback<GeometryChangedEvent> evt) { Register(evt); return x; }
        public FluentElement<T> OnLayoutUpdate(Action evt)
        {
            Register<GeometryChangedEvent>(_ => evt()); return x;
        }


        public FluentElement<T> OnClick(Action up)
        {
            OnClick((_, __) => up()); return x;
        }
        public FluentElement<T> OnClick(Action up, MouseButton button)
        {
            OnClick((evt, c) => up(), (int)button); return x;
        }
        
        public FluentElement<T> OnClick(EventCallback<MouseUpEvent> up)
        {
            OnClick((evt, _) => up(evt)); return x;
        }
        public FluentElement<T> OnClick(EventCallback<MouseUpEvent> up, MouseButton button)
        {
            OnClick((evt, _) => up(evt), (int)button); return x;
        }

        public FluentElement<T> OnClick(Call<MouseUpEvent> up)
        {
            // LMB is used as default value
            OnClick(up, 0); return x;
        }
        public FluentElement<T> OnClick(Call<MouseUpEvent> up, MouseButton button)
        {
            OnClick(up, (int)button); return x;
        }
        
        FluentElement<T> OnClick(Call<MouseUpEvent> up, int button)
        {
            var isMouseOver = false;
            
            // TODO: Could be improved by avoiding extra enter leave events?
            Register<MouseEnterEvent>(evt => isMouseOver = true);
            Register<MouseLeaveEvent>(evt => isMouseOver = false);
            
            Register<MouseUpEvent>((evt, c) =>
            {
                if (button != -1 && evt.button != button)
                    return;
                
                if (isMouseOver)
                    up.Invoke(evt, c);
            });
            return x;
        }


        public FluentElement<T> Register<TEvent>(EventCallback<TEvent> evt, TrickleDown trickleDown = TrickleDown.NoTrickleDown) where TEvent : EventBase<TEvent>, new()
        {
            x.RegisterCallback(evt, trickleDown);
            return x;
        }
        
        public FluentElement<T> Register<TEvent>(Call<TEvent> c, TrickleDown trickleDown = TrickleDown.NoTrickleDown) where TEvent : EventBase<TEvent>, new()
        {
            var handler = new CallHandle
            {
                x = x,
                eventTypeId = EventBase<TEvent>.TypeId()
            };
            var callback = (EventCallback<TEvent>)(evt =>
            {
                handler.evt = evt;
                handler.totalCalls++;
                handler.trickle = trickleDown;
                
                c.Invoke(evt, handler);
            });
            handler.callback = callback;
            
            x.RegisterCallback(callback, trickleDown);

            if (EventStorage<TEvent>.Handlers.ContainsKey(c))
                EventStorage<TEvent>.Handlers[c] = handler;
            else
                EventStorage<TEvent>.Handlers.Add(c, handler);
            
            return x;
        }
        

        public FluentElement<T> Unregister<TEvent>(Call<TEvent> c) where TEvent : EventBase<TEvent>, new()
        {
            if (EventStorage<TEvent>.Handlers.TryGetValue(c, out var handler))
                Unregister(handler);
            return x;
        }
        
        public FluentElement<T> Unregister<TEvent>(EventCallback<TEvent> callback) where TEvent : EventBase<TEvent>, new()
        {
            x.UnregisterCallback(callback);
            return x;
        }
        
        public FluentElement<T> Unregister(CallHandle handle)
        {
            FluentUITK.UnregisterCallback(x, handle.eventTypeId, handle.callback, handle.trickle);
            return x;
        }
    }
}