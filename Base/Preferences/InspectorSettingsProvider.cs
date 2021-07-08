
using System.Reflection;
using AV.Inspector.Runtime;
using AV.UITK;
using HarmonyLib;
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
        
        static PropertyInfo settingsWindowInfo = AccessTools.Property(typeof(SettingsProvider), "settingsWindow");
        
        [SettingsProvider]
        public static SettingsProvider CreateProvider() => new InspectorSettingsProvider(UIPath, SettingsScope.User);

        InspectorSettingsProvider(string path, SettingsScope scope) : base(path, scope) {}
        
        
        public override void OnActivate(string searchContext, VisualElement root)
        {
            var window = (EditorWindow)settingsWindowInfo.GetValue(this);
            window.SetAntiAliasing(8);
            
            var prefs = InspectorPrefs.LoadFromUserData();

            var ui = new InspectorPrefsUI(prefs);
            
            root.Add(CreateBigTitle());
            root.Add(ui);
        }

        Label CreateBigTitle()
        {
            return new Label(Title).Fluent().FontSize(19).FontStyle(FontStyle.Bold).Grow(0).Padding(2, 9, 2).Margin(bottom: 2);
        }
    }
}