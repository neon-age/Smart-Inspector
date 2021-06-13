
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    // Not sure if it's worth it.. We might instead skip expensive header methods using Harmony
    internal class ComponentHeaderElement : VisualElement
    {
        private static UIResources UIResource => UIResources.Asset;
        
        private static Texture2D menuIcon = EditorUtils.GetEditorIcon("_Menu");

        private VisualElement editorElement;
        private Editor editor;
        private Object target;
        
        public ComponentHeaderElement(VisualElement editorElement, Editor editor)
        {
            AddToClassList("component-header");
            
            styleSheets.Add(UIResource.commonStyles);

            this.editorElement = editorElement;
            this.editor = editor;
            this.target = editor.target;
            
            var serializedObject = editor.serializedObject;
            var iterator = serializedObject.GetIterator();
            
            serializedObject.Update();

            var hasVisibleFields = iterator.hasVisibleChildren;
            var canBeEnabled = serializedObject.FindProperty("m_Enabled") != null;

            var foldout = new Foldout { value = IsExpanded(target) }.AddClass("header-foldout");
            var expandZone = foldout.Query<Toggle>().First();

            if (hasVisibleFields)
            {
                foldout.RegisterValueChangedCallback(evt =>
                {
                    var isExpanded = IsExpanded(target);
                    SetExpanded(target, !isExpanded);
                });
            }
            else
                foldout.Query("unity-checkmark").First().visible = false;

            var inspectorTitle = ObjectNames.GetInspectorTitle(target);
            var objectIcon = EditorUtils.GetObjectIcon(target);
            
            var enabledToggle = new Toggle { visible = canBeEnabled, bindingPath = "m_Enabled" }.AddClass("enabled-toggle");
            
            var iconElement = new VisualElement().SetImage(objectIcon).AddClass("small-icon");
            var titleElement = new TextElement { text = inspectorTitle }.AddClass("header-title");
            
            var headerItemsGUI = new IMGUIContainer(() => EditorUtils.DrawEditorHeaderItems(target));
            
            var pinButton = new VisualElement().SetImage(UIResource.pinIcon).AddClass("small-icon", "header-item");
            var optionsButton = new VisualElement().SetImage(menuIcon).AddClass("small-icon", "header-item");
            
            pinButton.RegisterCallback<ClickEvent>(evt => PinEditor());
            optionsButton.RegisterCallback<ClickEvent>(evt => DisplayContextMenu(evt.position));
            
            RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 1)
                    DisplayContextMenu(evt.mousePosition);
            });
            
            Add(foldout);
            expandZone.Add(iconElement);
            expandZone.Add(enabledToggle);
            expandZone.Add(titleElement);
            
            var headerItems = new VisualElement().AddClass("header-items-group");
            Add(headerItems);
            
            
            headerItems.Add(headerItemsGUI.AddClass("header-items-gui"));
            headerItems.Add(pinButton);
            //headerItems.Add(optionsButton);
            
            this.Bind(serializedObject);
        }

        private void PinEditor()
        {
            var editorsList = editorElement.parent;
            editorsList.Insert(0, editorElement);
        }

        private void DisplayContextMenu(Vector2 position)
        {
            EditorUtils.DisplayContextMenu(new Rect(position, new Vector2(16, 0)), editor.targets);
        }

        private static bool IsExpanded(Object target)
        {
            return InternalEditorUtility.GetIsInspectorExpanded(target);
        }
        private static void SetExpanded(Object target, bool value)
        {
            InternalEditorUtility.SetIsInspectorExpanded(target, value);
        }
    }
}