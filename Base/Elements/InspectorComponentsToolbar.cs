
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorComponentsToolbar : InspectorElement
    {
        static UIResources UIResource => UIResources.Asset;

        readonly VisualElement tabsList = new VisualElement().AddClass("tabs-list");

        public InspectorComponentsToolbar()
        {
            Add(tabsList);
            AddToClassList("components-toolbar");
            
            styleSheets.Add(UIResource.commonStyles);
            styleSheets.Add(UIResource.componentsToolbarStyle);

            Rebuild();
            SwitchEditorTabs();
        }

        public void Rebuild()
        {
            tabsList.Clear();

            foreach (var editor in Inspector.editors)
            {
                var target = editor.editor.target;
                
                if (!target)
                    continue;

                var button = new InspectorEditorTab(editor);
                
                tabsList.Add(button);
                
                button.RegisterCallback<ChangeEvent<bool>>(_ => SwitchEditorTabs());
            }
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