

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal class InspectorPrefs : ScriptableObject
    {
        const string PrefsPath = "Open Labs/Smart Inspector";
        const string AssetName = "Smart Inspector Prefs.asset";
        static string LibraryPath => Path.Combine(Application.dataPath.Remove(Application.dataPath.Length - 7, 7), "Library", AssetName);
        
        [Serializable] public class PatchesTable : SerializedTable<string, PatchBase.State> {}

        public static InspectorPrefs Loaded
        {
            get => loaded ?? LoadFromUserData(); private set => loaded = value;
        }
        static InspectorPrefs loaded;
        
        [SerializeField] internal bool enablePlugin = true;
        [SerializeField] internal PatchesTable patchesTable = new PatchesTable();
        
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
        
        public bool useIMGUICulling => enhancements.imguiCulling && enabled;
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
            public bool showHelp = false;
            public bool showPresets = true;
            public bool showMenu = false;
        }
        [Serializable]
        public class Components
        {
            public bool showScript = false;
            public bool showScriptField = false;
        }
        [Serializable]
        public class Enhancements
        {
            public bool imguiCulling = true;
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
            public bool assetLabels = false;
            public bool assetBundle = false;
        }
        
        void Load(string json) => EditorJsonUtility.FromJsonOverwrite(json, this);


        public string SaveToUserData()
        {
            var json = EditorJsonUtility.ToJson(this, true);
            
            File.WriteAllText(LibraryPath, json);
            EditorPrefs.SetString(PrefsPath, json);
            
            return json;
        }
        
        public static InspectorPrefs LoadFromUserData()
        {
            if (!loaded)
                loaded = CreateInstance<InspectorPrefs>();

            string json = "";

            if (File.Exists(LibraryPath))
                json = File.ReadAllText(LibraryPath);
            else
                json = EditorPrefs.GetString(PrefsPath);
            
            loaded.Load(json);
            return loaded;
        }
    }
}