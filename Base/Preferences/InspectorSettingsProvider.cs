
using AV.Inspector.Runtime;
using UnityEditor;
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
            var serializedObject = new SerializedObject(prefs);

            var ui = new InspectorPrefsUI(serializedObject).Fluent();
            
            var scrollView = new ScrollView();
            scrollView.Add(ui);
            
            root.Add(CreateBigTitle());
            root.Add(scrollView);

            ui.Register<ChangeEvent<bool>>(_ => prefs.SaveToRegistry());
        }

        Label CreateBigTitle()
        {
            return new Label(Title).Fluent().FontSize(19).FontStyle(FontStyle.Bold).FlexGrow(0).Padding(2, 9, 2).Margin(bottom: 5);
        }
    }
}