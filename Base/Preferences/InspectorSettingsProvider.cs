
using AV.Inspector.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorSettingsProvider : SettingsProvider
    {
        const string Title = "Smart Inspector";
        const string UIPath = "Preferences/Open Labs/Smart Inspector";
        
        [SettingsProvider]
        public static SettingsProvider CreateProvider() => new InspectorSettingsProvider(UIPath, SettingsScope.User);

        InspectorSettingsProvider(string path, SettingsScope scope) : base(path, scope) {}
        
        
        public override void OnActivate(string searchContext, VisualElement root)
        {
            var prefs = InspectorPrefs.LoadFromRegistry();

            var ui = new InspectorPrefsUI(prefs).Fluent();
            
            var scrollView = new ScrollView();
            scrollView.Add(ui);
            
            root.Add(CreateBigTitle());
            root.Add(scrollView);

            var wasJustActivated = true;
            EditorApplication.delayCall += () => wasJustActivated = false;
            
            ui.Register<ChangeEvent<bool>>(SaveAndRebuildInspectors);
            
            void SaveAndRebuildInspectors<TValue>(ChangeEvent<TValue> evt)
            {
                // ChangeEvent is called when PropertyFields are just being created, we must avoid that
                if (wasJustActivated)
                    return;
                /*
                var x = evt.target.Fluent();
                if (!x.parent.HasClass(InspectorPrefsUI.RequiresRebuildClass))
                    return;*/
                
                prefs.SaveToRegistry();
                PropertyEditorRef.RebuildAllInspectors();
            }
        }

        Label CreateBigTitle()
        {
            return new Label(Title).Fluent().FontSize(19).FontStyle(FontStyle.Bold).FlexGrow(0).Padding(2, 9, 2).Margin(bottom: 5);
        }
    }
}