

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
        
        public bool enablePlugin = true;
        public bool showTabsBar = true;
        
        public Components components;
        public Enhancements enhancements;
        public AdditionalButtons additionalButtons;
        

        [Serializable]
        public class Components
        {
            public bool showPresets = true;
            public bool showHelp;
            public bool showMenu;
        }
        [Serializable]
        public class Enhancements
        {
            public bool betterComponentDragging;
            [Space]
            public bool compactPrefabInspector;
            public bool compactUnityEvents;
        }
        [Serializable]
        public class AdditionalButtons
        {
            public bool addressable;
            public bool convertToEntity;
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