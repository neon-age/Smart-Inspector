
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal static class UIToolkitFluentAPI
    {
        public static T This<T>(this T element, Action<VisualElement> action) where T : VisualElement
        {
            //action.Invoke(new Fluent { element = element });
            action.Invoke(element);
            return element;
        }
        
        #region Styling / Properties
        public static T SetName<T>(this T target, string name) where T : VisualElement
        {
            target.name = name;
            return target;
        }
        public static T AddClass<T>(this T target, string className) where T : VisualElement
        {
            target.AddToClassList(className);
            return target;
        }
        public static T AddClass<T>(this T target, params string[] classes) where T : VisualElement
        {
            foreach (var className in classes)
                target.AddToClassList(className);
            return target;
        }
        public static T SetNameAndClass<T>(this T target, string name, string className) where T : VisualElement
        {
            target.name = name;
            target.AddToClassList(className);
            return target;
        }
        
        public static T SetFlexGrow<T>(this T target, StyleFloat value) where T : VisualElement
        {
            target.style.flexGrow = value;
            return target;
        }
        public static T SetImage<T>(this T target, StyleBackground image) where T : VisualElement
        {
            target.style.backgroundImage = image;
            return target;
        }
        public static T SetSize<T>(this T target, StyleLength width = default, StyleLength height = default) where T : VisualElement
        {
            if (width != default) target.style.width = width;
            if (height != default) target.style.height = height;
            return target;
        }
        public static T SetSize<T>(this T target, Vector2 size) where T : VisualElement
        {
            target.style.width = size.x;
            target.style.height = size.y;
            return target;
        }
        #endregion

        #region UQuery
        public static bool HasAny<T>(this UQueryBuilder<T> query) where T : VisualElement
        {
            return query.First() != null;
        }
        public static VisualElement Get(this VisualElement target, string nameOrClass)
        {
            if (nameOrClass.StartsWith("."))
                return target.Query(className: nameOrClass.TrimStart('.'));
            
            if (nameOrClass.StartsWith("#"))
                return target.Query(name: nameOrClass.TrimStart('#'));
            
            return target.Query(nameOrClass);
        }
        #endregion
        
        #region Adding Elements
        public static VisualElement AddElement(this VisualElement target, int insert = -1)
        {
            var element = new VisualElement();
            target.Add(element);
            return element;
        }
        public static T AddNew<T>(this VisualElement target, int insert = -1) 
            where T : VisualElement, new()
        {
            var element = new T();
            
            target.Add(element);
            return element;
        }
        
        public static VisualElement AddImage(this VisualElement target, StyleBackground image, int insert = -1)
        {
            var element = new VisualElement().SetImage(image);
            AddOrInsertTo(target, element, insert);
            return element;
        }
        /*
        public static T AddIcon<T>(this T target, Texture2D icon, int insert = -1) where T : VisualElement
        {
            var element = new IconElement(icon);
            AddOrInsertTo(target, element, insert);
            return target;
        }*/

        private static void AddOrInsertTo(VisualElement target, VisualElement element, int insert = -1)
        {
            if (insert == -1) 
                target.Add(element);
            else
                target.Insert(insert, element);
        }
        #endregion
    }
}