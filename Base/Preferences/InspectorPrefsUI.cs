
using AV.Inspector.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorPrefsUI : VisualElement
    {
        internal const string RequiresRebuildClass = "requires-rebuild";
        
        StyleSheet styleSheet = Runtime.SmartInspector.FindStyleSheet(guid: "6932d1ef107ed0d49a54fb25d3699c9c");

        SerializedObject serialized;
        
        
        public InspectorPrefsUI(InspectorPrefs prefs)
        {
            this.serialized = new SerializedObject(prefs);
            CreateUI();
        }

        void CreateUI()
        {
            var me = this.Fluent();
            
            me.Bind(serialized);
            me.RegisterFewShots<GeometryChangedEvent>(_ => OnFirstLayoutChanges(), shots: 2);

            var topBar = me.NewHorizontalGroup().AddClass("top-bar", "separator").Add(
                
                Hyperlink("Github", PackageInfo.RepoURL),
                Bullet(),
                Hyperlink("Patreon", PackageInfo.PatreonURL),
                Bullet(),
                Hyperlink("Discord", PackageInfo.OpenLabsDiscordURL),
                
                me.NewFlexibleSpace(),
                
                SaveAsButton(),
                LoadFromButton()
                );

            var topGroup = me.NewIndentedGroup().AddClass("top-group", "separator");
            
            var enablePlugin = me.NewField<InspectorPrefs>(x => x.enablePlugin);
            var showTabsBar = me.NewField<InspectorPrefs>(x => x.showTabsBar);

            topGroup.Add(enablePlugin);
            topGroup.Add(showTabsBar);

            var headers = me.NewFoldout("Headers").Add(
                me.NewField<InspectorPrefs>(x => x.headers.showScript, "Show (Script)"),
                me.NewField<InspectorPrefs>(x => x.headers.showHelp),
                me.NewField<InspectorPrefs>(x => x.headers.showPresets),
                me.NewField<InspectorPrefs>(x => x.headers.showMenu)
                );
            var enhancements = me.NewFoldout("Quality Of Life").Add(
                me.NewField<InspectorPrefs>(x => x.enhancements.compactUnityEvents),
                me.NewField<InspectorPrefs>(x => x.enhancements.compactPrefabInspector).AddClass(RequiresRebuildClass),
                me.NewField<InspectorPrefs>(x => x.enhancements.compactScrollbar),
                me.NewField<InspectorPrefs>(x => x.enhancements.smarterComponentDragging, "Improved Component Dragging")
            );
            var additionalButtons = me.NewFoldout("Additional Buttons").Add(
                me.NewField<InspectorPrefs>(x => x.additionalButtons.addressable),
                me.NewField<InspectorPrefs>(x => x.additionalButtons.convertToEntity),
                me.NewField<InspectorPrefs>(x => x.additionalButtons.assetLabels),
                me.NewField<InspectorPrefs>(x => x.additionalButtons.assetBundle)
            );

            me.Add(topBar);
            me.Add(topGroup);
            me.Add(headers);
            me.Add(enhancements);
            me.Add(additionalButtons);

            styleSheets.Add(styleSheet);
            
            VisualElement Bullet() => me.NewText("•").FlexShrink(0).FontSize(8);
        }

        Button SaveAsButton()
        {
            return new Button { text = "Save As..." };
        }
        Button LoadFromButton()
        {
            return new Button { text = "Load From..." };
        }
        
        Label Hyperlink(string text, string url)
        {
            var link = new Label(text).Fluent().RegisterClick(_ => Application.OpenURL(url)).AddClass("hyperlink");
            
            link.TextOverflow(TextOverflow.Ellipsis);
            link.x.tooltip = url;
            
            return link;
        }

        void OnFirstLayoutChanges()
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