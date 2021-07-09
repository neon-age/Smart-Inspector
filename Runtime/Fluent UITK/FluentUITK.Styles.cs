using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

namespace AV.UITK
{
    [Flags]
    public enum Styles
    {
        None = 0,
        /// <summary> Adds ".unity-button" class of <see cref="UnityEngine.UIElements.Button"/>. </summary>
        Button = 2,
        /// <summary> Adds ".tab" class, used by ".tabs-bar" </summary>
        Tab = 4,
        /// <summary> Adds ".blue" class - checked <see cref="FluentUITK.Tab"/> will appear blue. </summary>
        Blue = 8,
        /// <summary> Moves <see cref="Toggle"/>'s checkbox before label.
        /// <para/> Adds ".toggle-left" class. </summary>
        ToggleLeft = 16,
        Separator = 32,
        // TODO: FoldoutHeader style

        ButtonBlue = Button | Blue,
        TabBlue = Tab | Blue,
    }
    
    public static partial class FluentUITK
    {
        /// <summary> Setup pre-defined styles to VisualElement. </summary>
        public static void DefineStyle(VisualElement x, Styles styles)
        {
            bool Has(Styles other) => (styles & other) != 0;

            if (Has(Styles.Button))
                x.AddToClassList("unity-button");
            
            if (Has(Styles.Tab))
                x.AddToClassList(TabClass);
            
            if (Has(Styles.Blue))
                x.AddToClassList("blue");

            if (Has(Styles.Separator))
                x.AddToClassList("separator");
            
            if (Has(Styles.ToggleLeft))
            {
                x.AddToClassList("toggle-left");
                if (x is Toggle)
                    x.Query(className: "unity-toggle__input").ForEach(e => x.Insert(0, e));
            }
        }
    }
}