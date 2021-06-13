
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class UIResources : ScriptableObject
    {
        public static UIResources Asset => Index.Asset.UIResources;
        
        //public static StyleSheet CommonStyles = AssetUtils.LoadFromGUID<StyleSheet>("ef1237b070c462e4eb26fca0332104a3");

        [Header("Style Sheets")]
        public StyleSheet commonStyles;
        public StyleSheet scrollViewStyle;
        public StyleSheet componentsHeaderStyle;
        public StyleSheet componentsToolbarStyle;
        public StyleSheet inspectorMainToolbarStyle;
        
        [Header("Icons")]
        public Texture2D pinIcon;
        public Texture2D debugIcon;
    }
}