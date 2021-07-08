
using AV.Inspector.Runtime;
using AV.UITK;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class InspectorPrefsUI : VisualElement
    {
        StyleSheet styleSheet = FluentUITK.FindStyleSheet(guid: "6932d1ef107ed0d49a54fb25d3699c9c");
        StyleSheet stylesLight = FluentUITK.FindStyleSheet(guid: "3ebaa7bab52db3c40b0457ab4ab089a8");
        
        InspectorPrefs prefs;
        SerializedObject serialized;
        bool wasJustCreated;
        
        FluentElement<InspectorPrefsUI> ui;
        FluentUITK.Tab enablePlugin;

        FluentElement<VisualElement> optionsPanel;
        FluentElement<VisualElement> patchesPanel;
        
        
        public InspectorPrefsUI(InspectorPrefs prefs)
        {
            this.prefs = prefs;
            this.serialized = new SerializedObject(prefs);
            
            styleSheets.Add(styleSheet);
            if (!FluentUITK.isProSkin)
                styleSheets.Add(stylesLight);
            
            ui = this.Fluent();
            ui.OnChange<bool>(OnChange);

            optionsPanel = CreateOptionsPanel();
            patchesPanel = new InspectorPatchesUI();

            var scrollView = new ScrollView().Fluent().Shrink(-100).Add(
                optionsPanel,
                patchesPanel
                );
            
            ui.Add(
                CreateTopPanel(),
                scrollView
            );
            
            ui.Bind(serialized);
        }

        void SaveAndRebuildInspectors()
        {
            prefs.SaveToUserData();
            PropertyEditorRef.RebuildAllInspectors();
        }

        VisualElement CreateTabsBar()
        {
            var me = ui.NewTabsBar(Styles.Button);
            
            me.x.AddNewTab(optionsPanel, "Options").Out(out var optionsTab);
            me.x.AddNewTab(patchesPanel, "Patches");

            me.OnAttach(optionsTab.x.Show);
            return me;
        }

        VisualElement CreateTopPanel()
        {
            var me = ui.NewGroup().Name("Top Panel");
            
            var topBar = me.NewRow().AddClass("top-bar").Add(
                
                me.NewHyperlink("Github", PackageInfo.RepoURL),
                me.NewBullet(),
                me.NewHyperlink("Patreon", PackageInfo.PatreonURL),
                me.NewBullet(),
                me.NewHyperlink("Discord", PackageInfo.OpenLabsDiscordURL),
                
                me.NewFlexibleSpace(),
                
                PresetsButton()
            );

            var topGroup = me.NewRow().Margin(left: 6, right: 4).AddClass("top-group").Add(
                
                CreateTabsBar(),
                
                enablePlugin = me.NewTab(style: Styles.ButtonBlue)
                    .Grow(1)
                    .Bind<InspectorPrefs>(x => x.enablePlugin)
            );
            SetEnablePluginState(prefs.enabled);

            me.Add(topBar);
            me.Add(topGroup);
            return me;
        }

        VisualElement CreateOptionsPanel()
        {
            wasJustCreated = true;
            EditorApplication.delayCall += () => wasJustCreated = false;

            var me = ui.NewGroup().Name("Options");
            
            me.OnLayoutUpdate((_, c) =>
            {
                if (c.totalCalls > 1)
                    c.Unregister();
                OnFirstLayoutUpdates();
            });

            var toolbar = me.NewHeader("Toolbar").Add(
                me.NewField<InspectorPrefs>(x => x.toolbar.showTabsBar, "Components Toolbar")
            );
            var headers = me.NewHeader("Titlebars").Add(
                me.NewField<InspectorPrefs>(x => x.headers.showHelp),
                me.NewField<InspectorPrefs>(x => x.headers.showPresets),
                me.NewField<InspectorPrefs>(x => x.headers.showMenu).Enabled(false)
                );
            var components = me.NewHeader("Components").Add(
                me.NewField<InspectorPrefs>(x => x.components.showScript, "Show (Script)"),
                me.NewField<InspectorPrefs>(x => x.components.showScriptField)
            );
            var enhancements = me.NewHeader("Enhancements").Add(
                #if UNITY_2020_1_OR_NEWER
                me.NewField<InspectorPrefs>(x => x.enhancements.imguiCulling, "IMGUI Culling"),
                #endif
                me.NewField<InspectorPrefs>(x => x.enhancements.compactUnityEvents).Enabled(false),
                me.NewField<InspectorPrefs>(x => x.enhancements.compactPrefabInspector),
                me.NewField<InspectorPrefs>(x => x.enhancements.compactScrollbar),
                me.NewField<InspectorPrefs>(x => x.enhancements.smoothScrolling),
                me.NewField<InspectorPrefs>(x => x.enhancements.smartComponentDragging, "Improved Component Dragging")
            );
            var additionalButtons = me.NewHeader("Additional Buttons").Add(
                me.NewField<InspectorPrefs>(x => x.additionalButtons.addressable),
                me.NewField<InspectorPrefs>(x => x.additionalButtons.convertToEntity),
                me.NewField<InspectorPrefs>(x => x.additionalButtons.assetLabels),
                me.NewField<InspectorPrefs>(x => x.additionalButtons.assetBundle)
            );

            me.Add(toolbar);
            
            me.Add(headers);
            me.Add(components);
            me.Add(enhancements);
            me.Add(additionalButtons);
            
            me.Add(me.NewSpace(height: 20));

            me.x.Query<FluentUITK.Header>().ForEach(x => x.Fluent().Style(Styles.Separator));
            
            return me;
        }
        
        void OnChange<TValue>(ChangeEvent<TValue> evt)
        {
            if (wasJustCreated)
                return;
            
            var e = evt.target as VisualElement;
            var boolEvt = evt as ChangeEvent<bool>;

            if (e == enablePlugin)
            {
                var patch = boolEvt.newValue;
                prefs.enablePlugin = patch;
                
                SetEnablePluginState(true, patch ? "Patching..." : "Unpatching...");
                enablePlugin.MarkDirtyRepaint();
                
                EditorApplication.delayCall += () =>
                {
                    SetEnablePluginState(patch);
                    PatchOrUnpatchAll(patch);
                };
            }
            else
            {
                SaveAndRebuildInspectors();
            }
        }

        void PatchOrUnpatchAll(bool patch)
        {
            if (patch)
            {
                Patcher.ApplyPatches();
                SaveAndRebuildInspectors();
            }
            else
            {
                SaveAndRebuildInspectors();
                Patcher.UnpatchAll();
            }
        }
        
        void SetEnablePluginState(bool enabled, string text = null)
        {
            enablePlugin.text = text ?? (enabled ? "Disable Plugin" : "Enable Plugin");
            
            this.Query<FluentUITK.Header>().ForEach(x => x.container.SetEnabled(enabled));
        }

        Button PresetsButton()
        {
            var icon = FluentUITK.GetEditorIcon("Preset.Context");
            return ui.NewButton(icon: icon).Style(Styles.Tab).OnClick(_ => EditorUtils.Presets.ShowMenu(prefs));
        }

        void OnFirstLayoutUpdates()
        {
            this.Query<Foldout>().ForEach(TurnFoldoutIntoHeader);
            ui.x.Query<Toggle>().ForEach(e => e.Fluent().Style(Styles.ToggleLeft));
        }
        
        void TurnFoldoutIntoHeader(Foldout x)
        {
            x.value = true;
            x.RemoveFromClassList("unity-foldout");

            x.Query<Toggle>().First().pickingMode = PickingMode.Ignore;
            x.Query<Toggle>().Children<VisualElement>().ForEach(e => e.pickingMode = PickingMode.Ignore);
        }
    }
}