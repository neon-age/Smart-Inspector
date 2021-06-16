using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal static class VisualElementUtils
    {
        static readonly PropertyInfo worldClip = AccessTools.Property(typeof(VisualElement), "worldClip");
        
        public static Rect GetWorldClip(this VisualElement element)
        {
            return (Rect)worldClip.GetValue(element);
        }
    }
}