

using System;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal class InspectorPrefs : ScriptableObject
    {
        const string PrefsPath = "Open Labs/Smart Inspector";

        public static InspectorPrefs Loaded
        {
            get => loaded ?? LoadFromRegistry(); private set => loaded = value;
        }
        static InspectorPrefs loaded;
        
        [SerializeField] internal bool enablePlugin = true;
        
        [SerializeField] internal Toolbar toolbar = new Toolbar();
        [SerializeField] internal Headers headers = new Headers();
        [SerializeField] internal Components components = new Components();
        [SerializeField] internal Enhancements enhancements = new Enhancements();
        [SerializeField] internal AdditionalButtons additionalButtons = new AdditionalButtons();
        
        public bool enabled => enablePlugin;
        public bool showTabsBar => toolbar.showTabsBar && enabled;
        
        public bool showHelp => !enabled || headers.showHelp;
        public bool showPreset => !enabled || headers.showPresets;
        public bool showMenu => !enabled || headers.showMenu;
        
        public bool showScript => components.showScript && enabled;
        public bool showScriptField => components.showScriptField && enabled;
        
        public bool useCompactPrefabInspector => enhancements.compactPrefabInspector && enabled;
        public bool useCompactScrollbar => enhancements.compactScrollbar && enabled;
        public bool useSmoothScrolling => enhancements.smoothScrolling && enabled;
        public bool useSmartDrag => enhancements.smartComponentDragging && enabled;
        
        public bool showAddressable => !enabled || additionalButtons.addressable;
        public bool showConvertToEntity => !enabled || additionalButtons.convertToEntity;
        public bool showLabel => !enabled || additionalButtons.assetLabels;
        public bool showBundle => !enabled || additionalButtons.assetBundle;

        
        [Serializable]
        public class Toolbar
        {
            public bool showTabsBar = true;
        }
        [Serializable]
        public class Headers
        {
            public bool showPresets = true;
            public bool showHelp;
            public bool showMenu;
        }
        [Serializable]
        public class Components
        {
            public bool showScript;
            public bool showScriptField;
        }
        [Serializable]
        public class Enhancements
        {
            public bool compactPrefabInspector= true;
            public bool compactUnityEvents = true;
            public bool compactScrollbar = true;
            public bool smoothScrolling = true;
            public bool smartComponentDragging = true;
        }
        [Serializable]
        public class AdditionalButtons
        {
            public bool addressable = true;
            public bool convertToEntity = true;
            public bool assetLabels;
            public bool assetBundle;
        }

        
        public string SaveToRegistry()
        {
            var json = EditorJsonUtility.ToJson(this, true);
            EditorPrefs.SetString(PrefsPath, json);
            return json;
        }

        public void Load(string json)
        {
            EditorJsonUtility.FromJsonOverwrite(json, this);
        }
        
        public static InspectorPrefs LoadFromRegistry()
        {
            if (!loaded)
                loaded = CreateInstance<InspectorPrefs>();
            
            var json = EditorPrefs.GetString(PrefsPath);
            Loaded.Load(json);
            return Loaded;
        }
    }
}