
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorComponentsToolbar : InspectorElement
    {
        private static UIResources UIResource => UIResources.Asset;

        public InspectorComponentsToolbar()
        {
            var editors = Inspector.editors;
            
            AddToClassList("components-toolbar");
            
            styleSheets.Add(UIResource.commonStyles);
            styleSheets.Add(UIResource.componentsToolbarStyle);

            //var localHierarchyButton = CreateToggle(false, hierarchyIcon);
            //Add(localHierarchyButton);
            
            //var goButton = CreateToggle(false, EditorUtils.GetObjectIcon(inspectedObject));
            //Add(goButton);

            //var components = gameObject.GetComponents<Component>();
            foreach (var editor in editors)
            {
                var target = editor.editor.target;
                
                if (!target)
                    continue;

                var button = new InspectorEditorTab(editor);
                
                Add(button);
                
                button.RegisterCallback<ChangeEvent<bool>>(_ => SwitchEditorTabs());
            }

            SwitchEditorTabs();
        }

        void SwitchEditorTabs()
        {
            var tabs = this.Query<InspectorEditorTab>();

            var isAnyActive = false;
            tabs.ForEach(x => {
                if (x.value)
                    isAnyActive = true;
            });

            tabs.ForEach(x =>
            {
                var show = isAnyActive ? x.value : true;
                
                x.editor.element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            });
        }
    }
}