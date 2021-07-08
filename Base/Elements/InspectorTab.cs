using System;
using System.Threading.Tasks;
using AV.Inspector.Runtime;
using AV.UITK;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using EditorElement = AV.Inspector.Runtime.SmartInspector.EditorElement;

namespace AV.Inspector
{
    internal class InspectorTab : ToolbarToggle
    {
        const string EditorTabClass = "editor-tab";
        const string TabStateKeyPrefix = "tab ";

        [Flags]
        public enum State
        {
            None = 0,
            IsActive = 1,
            WasExpanded = 2, // Was inspector expanded before tab activation?
        }
        
        public readonly EditorElement editor;
        
        readonly SmartInspector inspector = SmartInspector.RebuildingInspector;
        readonly ActiveEditorTracker tracker;

        Texture2D preview;
        Texture2D thumbnail;
        FluentUITK.Icon icon;

        Object target => editor.target;
        
        
        public InspectorTab(EditorElement editor)
        {
            this.editor = editor;
            this.tracker = PropertyEditorRef.GetTracker(inspector.propertyEditor);

            name = GetTitle();
            RestoreTabState();

            AddToClassList(EditorTabClass);
            AddToClassList("toolbar-button");

            RegisterCallbacks();
            
            preview = AssetPreview.GetAssetPreview(target);
            thumbnail = AssetPreview.GetMiniThumbnail(target);
            
            icon = new FluentUITK.Icon();
            Add(icon);
            
            this.Query(className: "unity-toggle__input").First().RemoveFromHierarchy();
        }

        void RegisterCallbacks()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
            RegisterCallback<ChangeEvent<bool>>(evt => SetActive(evt.newValue));
            
            #if !UNITY_2020_2_OR_NEWER 
            // Toggle value doesn't change in 2019.4 and 2020.1 for some reason (??), so do it manually
            RegisterCallback<MouseUpEvent>(evt => value = !value);
            #endif

            RegisterCallback<MouseEnterEvent>(evt => inspector.tooltip.ShowAt(worldBound, name, TooltipElement.Align.Top));
            RegisterCallback<MouseLeaveEvent>(evt => inspector.tooltip.Hide());
        }

        void RestoreTabState()
        {
            var state = LoadState();
            var wasActive = (state & State.IsActive) != 0;
            
            value = wasActive;
            
            if (wasActive)
                SetInspectorExpanded(true);
        }

        State LoadState()
        {
            return (State)PackagePrefs.Get<int>(GetPrefKey());
        }
        State GetState(bool active, bool expanded)
        {
            var state = State.None;
            
            if (active) state |= State.IsActive;
            if (expanded) state |= State.WasExpanded;
            
            return state;
        }

        void SetInspectorExpanded(bool expand)
        {
            SetVisible(expand);
            // Internally serialized expanding, needs to be restored on deactivation.
            // We use it here to show Gizmos temporarily.
            InternalEditorUtility.SetIsInspectorExpanded(target, expand); 
        }
        // Non-serialized expanding
        void SetVisible(bool visible)
        {
            var index = EditorElementRef.GetEditorIndex(editor.element);
            
            tracker.SetVisible(index, visible ? 1 : 0);
            editor.inspector.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void SetActive(bool active)
        {
            var state = LoadState();
            var wasExpanded = (state & State.WasExpanded) != 0;
            
            if (!active)
            {
                SetInspectorExpanded(wasExpanded); // Restore previous expanding state
            }
            
            if (active)
            {
                wasExpanded = InternalEditorUtility.GetIsInspectorExpanded(target);
                SetInspectorExpanded(true); // Always expand on tab activation
            }

            state = GetState(active, wasExpanded);
            
            PackagePrefs.Set<int>(GetPrefKey(), (int)state);
            
            inspector.propertyEditor.Repaint();
        }

        void OnGeometryChange(GeometryChangedEvent evt)
        {
            if (!preview)
                preview = AssetPreview.GetAssetPreview(target);
            
            icon.image = preview ? preview : thumbnail;
        }
        
        
        string GetStateLog()
        {
            return $"{LoadState()} {GetPrefKey()}";
        }

        string GetPrefKey()
        {
            string key = "";

            if (target is Component)
            {
                key = target.GetType().FullName;
            }
            // Use guid for assets (usually Materials)
            else if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(target, out var guid, out long localId))
            {
                key = guid;
            }

            return TabStateKeyPrefix + key;
        }
        
        string GetTitle()
        {
            var title = target.name;
            
            if (target is Component component)
                title = SmartInspector.GetInspectorTitle(component);
            
            return title;
        }
    }
}