
using AV.Inspector.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorPrefsUI : VisualElement
    {
        StyleSheet styleSheet = Runtime.SmartInspector.FindStyleSheet(guid: "6932d1ef107ed0d49a54fb25d3699c9c");
        
        SerializedObject serialized;
        
        
        public InspectorPrefsUI(SerializedObject serialized)
        {
            this.serialized = serialized;
            CreateUI();
        }

        void CreateUI()
        {
            var me = this.Fluent();
            
            me.Bind(serialized);
            me.RegisterOneShot<GeometryChangedEvent>(_ => OnFirstLayoutChange());

            var linksBar = me.NewHorizontalGroup().AddClass("links-bar", "separator");

            linksBar.Add(GithubLink());

            var topGroup = me.NewIndentedGroup().AddClass("top-group", "separator");
            var enablePlugin = me.NewProperty(nameof(InspectorPrefs.enablePlugin));
            var showTabsBar = me.NewProperty(nameof(InspectorPrefs.showTabsBar));

            topGroup.Add(enablePlugin);
            topGroup.Add(showTabsBar);
            
            var components = me.NewProperty(nameof(InspectorPrefs.components));
            var enhancements = me.NewProperty(nameof(InspectorPrefs.enhancements));
            var additionalButtons = me.NewProperty(nameof(InspectorPrefs.additionalButtons));

            me.Add(linksBar);
            me.Add(topGroup);
            me.Add(components);
            me.Add(enhancements);
            me.Add(additionalButtons);

            styleSheets.Add(styleSheet);
        }

        Label GithubLink()
        {
            return new Label("Github Page").Fluent().RegisterClick(_ => Application.OpenURL(PackageInfo.RepoURL)).AddClass("hyperlink");
        }

        void OnFirstLayoutChange()
        {
            this.Query<Label>(className: "unity-property-field__label").ForEach(ExtendLabel);
            this.Query<Foldout>().ForEach(TurnFoldoutIntoHeader);
        }

        void ExtendLabel(Label x)
        {
            x.style.minWidth = 200;
        }
        void TurnFoldoutIntoHeader(Foldout x)
        {
            x.value = true;
            x.AddToClassList("separator");
            x.RemoveFromClassList("unity-foldout");

            x.Query<Toggle>().First().pickingMode = PickingMode.Ignore;
            x.Query<Toggle>().Children<VisualElement>().ForEach(e => e.pickingMode = PickingMode.Ignore);
        }
    }
}