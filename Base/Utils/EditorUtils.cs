using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal static class EditorUtils
    {
        static MethodInfo drawHeaderItems = typeof(EditorGUIUtility).GetMethod("DrawEditorHeaderItems", BindingFlags.NonPublic | BindingFlags.Static);
        static MethodInfo displayContextMenu = typeof(EditorUtility).GetMethod("DisplayObjectContextMenu", BindingFlags.NonPublic | BindingFlags.Static, null, 
            new [] { typeof(Rect), typeof(Object[]), typeof(int) }, null);
        
        static Texture2D fileIcon = GetEditorIcon("File");
        static Texture2D warningIcon = GetEditorIcon("console.warnicon");
        
        public static Texture2D GetEditorIcon(string iconName)
        {
            return EditorGUIUtility.IconContent(iconName).image as Texture2D;
        }
        
        public static Texture2D GetObjectIcon(Object obj)
        {
            if (obj == null)
                return fileIcon;
            
            return EditorGUIUtility.ObjectContent(obj, obj.GetType()).image as Texture2D;
        }

        public static void DisplayContextMenu(Rect rect, Object[] targets, int contextUserData = 0)
        {
            displayContextMenu.Invoke(null, new object[] { rect, targets, contextUserData });
        }
        
        public static void DrawEditorHeaderItems(params Object[] targets)
        {
            //var evt = Event.current;
            
            //var titleRect = GUILayoutUtility.GetRect(16, 16);
            //titleRect.x -= 20;
            var rect = new Rect(16, 0, 16, 16);
            var itemsRect = (Rect)drawHeaderItems.Invoke(null, new object[] { rect, targets, 2 /*spacing*/ });
            
            rect.xMin = itemsRect.x;
            GUILayoutUtility.GetRect(rect.width, 16);
            
            //displayContextMenu.Invoke(null, new object[] { optionsRect, targets, 0 });
        }
    }
}