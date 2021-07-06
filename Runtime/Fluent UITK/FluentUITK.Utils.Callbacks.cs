using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.UITK
{
    public static partial class FluentUITK
    {
        const BindingFlags AnyBinding = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        static Assembly EventAssembly { get; } = typeof(CallbackEventHandler).Assembly;
        
        internal enum CallbackPhase
        {
            TargetAndBubbleUp = 1 << 0,
            TrickleDownAndTarget = 1 << 1
        }
        
        /// <summary> Represents retrieved info about registered <see cref="EventCallback{TEventType}"/>. </summary>
        public class EventFunctor
        {
            public CallbackEventHandler handler { get; internal set; }
            public Delegate callback { get; internal set; }
            public long eventTypeId { get; internal set; }
            
            public TrickleDown trickleDown => phase == CallbackPhase.TrickleDownAndTarget
                ? TrickleDown.TrickleDown
                : TrickleDown.NoTrickleDown;

            internal CallbackPhase phase;
            internal object callbackRegistry;

            
            public bool Is<T>() => callback is EventCallback<T>;
            public bool Is<T>(out EventCallback<T> evt) where T : class
            {
                evt = callback as EventCallback<T>;
                return Is<T>();
            }

            public void Unregister()
            {
                EventCallbackRegistryRef.UnregisterCallback(callbackRegistry, eventTypeId, callback, trickleDown);
            }
        }

        public static void UnregisterCallback(CallbackEventHandler handler, long eventTypeId, Delegate callback, TrickleDown trickle)
        {
            var registry = CallbackEventHandlerRef.GetCallbackRegistry(handler);
            
            EventCallbackRegistryRef.UnregisterCallback(registry, eventTypeId, callback, trickle);
        }

        /// <summary> Returns info about all callbacks registered to the event handler. </summary>
        public static IEnumerable<EventFunctor> GetCallbacks(CallbackEventHandler handler)
        {
            var registry = CallbackEventHandlerRef.GetCallbackRegistry(handler);
            var callbackList = EventCallbackRegistryRef.GetCallbackListForWriting(registry);
            var list = EventCallbackListRef.GetList(callbackList);

            // Avoids modified collection error on unregister
            var tempList = new List<object>(list.Count);
            foreach (var item in list)
                tempList.Add(item);
            
            foreach (var functor in tempList)
            {
                var functorType = functor.GetType();
                
                if (!FunctorCache.infos.TryGetValue(functorType, out var info))
                {
                    info = new FunctorInfo(functorType);
                    FunctorCache.infos.Add(functorType, info);
                }

                // TODO: Should it be optimized with lambdas? It might eat up a chunk of memory in extreme cases.. Need some benchmarking.
                var callback = (Delegate)info.m_Callback.GetValue(functor);
                var eventTypeId = (long)info.m_EventTypeId.GetValue(functor);
                var phase = (CallbackPhase)info.phase.GetValue(functor);
                
                yield return new EventFunctor()
                {
                    handler = handler,
                    callback = callback,
                    eventTypeId = eventTypeId,
                    
                    phase = phase,
                    callbackRegistry = registry,
                };
            }
        }

        class FunctorInfo
        {
            public PropertyInfo phase;
            public FieldInfo m_Callback;
            public FieldInfo m_EventTypeId;

            public FunctorInfo(Type functorType)
            {
                phase = functorType.GetProperty("phase", AnyBinding);
                m_Callback = functorType.GetField("m_Callback", AnyBinding);
                m_EventTypeId = functorType.GetField("m_EventTypeId", AnyBinding);
            }
        }
        static class FunctorCache
        {
            internal static Dictionary<Type, FunctorInfo> infos = new Dictionary<Type, FunctorInfo>();
        }

        static class EventCallbackListRef
        {
            static Type type { get; } = EventAssembly.GetType("UnityEngine.UIElements.EventCallbackList");

            static FieldInfo m_List = type.GetField("m_List", AnyBinding);

            static Func<object, IList> getListFunc;

            static EventCallbackListRef()
            {
                var objParam = Expression.Parameter(typeof(object));
                var convert = Expression.Convert(objParam, type);
                var field = Expression.Field(convert, m_List);

                getListFunc = Expression.Lambda<Func<object, IList>>(field, objParam).Compile();
            }
            
            internal static IList GetList(object eventCallbackList)
            {
                return getListFunc(eventCallbackList);
            }
        }
        
        static class CallbackEventHandlerRef
        {
            static FieldInfo m_CallbackRegistry = typeof(CallbackEventHandler).GetField("m_CallbackRegistry", AnyBinding);

            static Func<CallbackEventHandler, object> getCallbackRegistryFunc;

            static CallbackEventHandlerRef()
            {
                var param = Expression.Parameter(typeof(CallbackEventHandler));
                var field = Expression.Field(param, m_CallbackRegistry);
                
                getCallbackRegistryFunc = Expression.Lambda<Func<CallbackEventHandler, object>>(field, param).Compile();
            }

            internal static object GetCallbackRegistry(CallbackEventHandler handler)
            {
                return getCallbackRegistryFunc(handler);
            }
        }
        
        static class EventCallbackRegistryRef
        {
            static Type type { get; } = EventAssembly.GetType("UnityEngine.UIElements.EventCallbackRegistry");

            static MethodInfo unregisterCallback = type.GetMethod("UnregisterCallback", AnyBinding, null, new []
            {
                typeof(long), typeof(Delegate), typeof(TrickleDown)
            }, null);
            
            static MethodInfo getCallbackListForWriting = type.GetMethod("GetCallbackListForWriting", AnyBinding);

            static Action<object, long, Delegate, TrickleDown> unregisterCallbackFunc;
            static Func<object, object> getCallbackListFunc;
            
            
            static EventCallbackRegistryRef()
            {
                var objParam = Expression.Parameter(typeof(object));
                var longParam = Expression.Parameter(typeof(long));
                var delegateParam = Expression.Parameter(typeof(Delegate));
                var trickleParam = Expression.Parameter(typeof(TrickleDown));
                var convert = Expression.Convert(objParam, type);
                
                var unregisterCall = Expression.Call(convert, unregisterCallback, longParam, delegateParam, trickleParam);
                var getListCall = Expression.Call(convert, getCallbackListForWriting);

                unregisterCallbackFunc = Expression.Lambda<Action<object, long, Delegate, TrickleDown>>(unregisterCall, 
                    objParam, longParam, delegateParam, trickleParam).Compile();
                
                getCallbackListFunc = Expression.Lambda<Func<object, object>>(getListCall, objParam).Compile();
            }
            
            internal static void UnregisterCallback(object registry, long eventTypeId, Delegate callback, TrickleDown useTrickleDown)
            {
                unregisterCallbackFunc(registry, eventTypeId, callback, useTrickleDown);
            }
            
            internal static object GetCallbackListForWriting(object registry)
            {
                return getCallbackListFunc(registry);
            }
        }
    }
}