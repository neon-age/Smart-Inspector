
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.UITK
{
    public partial class FluentElement<T>
    {
        public bool Has<TType>() where TType : VisualElement
        {
            return Get<TType>() != null;
        }
        public bool Has<TType>(string nameOrClass) where TType : VisualElement
        {
            return Get<TType>(nameOrClass).First() != null;
        }
        public bool Has(string nameOrClass)
        {
            return Get(nameOrClass).First() != null;
        }

        public bool Has<TType>(out TType e) where TType : VisualElement
        {
            e = Get<TType>(); return e != null;
        }
        public bool Has<TType>(string nameOrClass, out TType e) where TType : VisualElement
        {
            e = Get<TType>(nameOrClass); return e != null;
        }
        public bool Has(string nameOrClass, out VisualElement e)
        {
            e = Get(nameOrClass); return e != null;
        }
        
        
        // TODO: UQueryBuilder wrapper?
        
        public UQueryBuilder<TType> Get<TType>() where TType : VisualElement
        {
            return x.Query<TType>();
        }
        public UQueryBuilder<VisualElement> Get(string nameOrClass)
        {
            return Get<VisualElement>(nameOrClass);
        }
        public UQueryBuilder<TType> Get<TType>(string nameOrClass) where TType : VisualElement
        {
            if (nameOrClass.StartsWith(".")) return x.Query<TType>(className: nameOrClass.TrimStart('.'));
            if (nameOrClass.StartsWith("#")) return x.Query<TType>(name: nameOrClass.TrimStart('#'));
            return x.Query<TType>(nameOrClass);
        }
    }
}