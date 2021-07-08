using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AV.Inspector.Runtime;
using HarmonyLib;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class PrefabImporterPatch
    {
        static InspectorPrefs prefs => InspectorPrefs.Loaded;
        
        static FieldInfo m_PrefabsWithMissingScript;

        
        [InitializeOnInspector]
        static void OnInspector()
        {
            var type = typeof(Editor).Assembly.GetType("UnityEditor.PrefabImporterEditor");

            m_PrefabsWithMissingScript = AccessTools.Field(type, "m_PrefabsWithMissingScript");
            
            Runtime.SmartInspector.OnSetupEditorElement += x =>
            {
                if (x.header.name != "Prefab ImporterHeader")
                    return;

                var missingScriptsList = (List<string>)m_PrefabsWithMissingScript.GetValue(x.editor);
                if (missingScriptsList.Count > 0)
                    return;
                
                var showCompact = prefs.useCompactPrefabInspector;

                x.Display(!showCompact);
                Hide();

                //x.header.x.onGUIHandler += Hide;

                // Hide vanilla prefab header, so we can make our own
                void Hide()
                {
                    x.header.Display(!showCompact);
                    x.inspector.Display(!showCompact);
                }
            };
        }
    }
}
