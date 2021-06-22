
using AV.Inspector.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorTabsBar : VisualElement
    {
        readonly VisualElement tabsList = new VisualElement();
        
        public InspectorTabsBar()
        {
            tabsList.AddToClassList("tabs-list");
            
            Add(tabsList);
            
            styleSheets.Add(SmartInspector.tabsBarStyles);
            
            if (!EditorGUIUtility.isProSkin)
                styleSheets.Add(SmartInspector.tabsBarStylesLight);
        }

        public void Rebuild(SmartInspector inspector)
        {
            this.Query<InspectorTab>().ForEach(x => x.RemoveFromHierarchy());
            
            
            foreach (var editor in inspector.editors.Values)
            {
                if (editor.element.HasClass("game-object"))
                    continue;
                
                var target = editor.editor.target;
                
                if (!target)
                    continue;

                var tab = new InspectorTab(editor);
                
                if (string.IsNullOrEmpty(tab.name))
                    continue;
                
                tabsList.Add(tab);
                
                tab.RegisterCallback<ChangeEvent<bool>>(_ => UpdateEditorsVisibility());
            }

            UpdateEditorsVisibility();
        }

        void UpdateEditorsVisibility()
        {
            var tabs = this.Query<InspectorTab>();
            
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