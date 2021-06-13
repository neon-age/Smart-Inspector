using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    // Dunno about this one, using it as top-toolbar looks bloaty.. And there is not much we can put inside
    internal class InspectorMainToolbar : VisualElement
    {
        private static UIResources UIResource => UIResources.Asset;
        
        private static Texture2D backIcon = EditorUtils.GetEditorIcon("back");
        private static Texture2D forwardIcon = EditorUtils.GetEditorIcon("forward");
        private static Texture2D favoriteIcon = EditorUtils.GetEditorIcon("Favorite");
        
        public InspectorMainToolbar(EditorWindow inspector)
        {
            AddToClassList("inspector-main-toolbar");
            
            styleSheets.Add(UIResource.commonStyles);
            styleSheets.Add(UIResource.inspectorMainToolbarStyle);
            
            var backButton = CreateButton(backIcon).This(e => e.Get(".small-icon").SetSize(9, 10));
            var forwardButton = CreateButton(forwardIcon).This(e => e.Get(".small-icon").SetSize(9, 10));
            
            var flexSpace = new VisualElement().SetFlexGrow(1);
            
            var favoriteButton = CreateButton(favoriteIcon);
            
            var isDebugMode = InspectorWindowRef.GetInspectorMode(inspector) == InspectorMode.Debug;
            var debugButton = CreateToggle(isDebugMode, UIResource.debugIcon);
            
            debugButton.RegisterValueChangedCallback(evt => InspectorWindowRef.SwitchDebugMode(inspector));
            
            Add(backButton);
            Add(forwardButton);
            ///////////////
            Add(flexSpace);
            Add(favoriteButton);
            Add(debugButton);
        }

        private ToolbarToggle CreateToggle(bool value, Texture2D icon)
        {
            return new ToolbarToggle { value = value }.AddClass("toolbar-button").This(e =>
            {
                e.Get(".unity-toggle__input").RemoveFromHierarchy();
                e.AddImage(icon).AddClass("small-icon");
            });
        }
        private ToolbarButton CreateButton(Texture2D icon)
        {
            return new ToolbarButton().AddClass("toolbar-button").This(e => e.AddImage(icon).AddClass("small-icon"));
        }
    }
}