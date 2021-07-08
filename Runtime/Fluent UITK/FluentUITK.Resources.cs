#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UIElements;

namespace AV.UITK
{
    public static partial class FluentUITK
    {
        public static bool isProSkin => 
            #if UNITY_EDITOR
            EditorGUIUtility.isProSkin;
            #else
            false;
            #endif
        
        internal static StyleSheet coreStyles => FindStyleSheet(guid: "a65295aa1e4ad0e418b38767b4b62004");
        internal static StyleSheet coreStylesLight => FindStyleSheet(guid: "26058bff54cdbee4fb77f501595a2baf");


        internal static void AddCoreStyles(VisualElement x)
        {
            x.styleSheets.Add(coreStyles);
            
            if (!isProSkin)
                x.styleSheets.Add(coreStylesLight);
        }
    }
}