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
    internal class PrefabImporterPatch : PatchBase
    {
        static PropertyInfo assetTargetInfo;
        static MethodInfo cacheHasMixedBaseInfo;
        static MethodInfo openPrefab;

        static GUIContent tempContent = new GUIContent();

        /*
        class PrefabAssetTarget
        {
            public Object assetTarget;
            public Object variantBase;
        }
        
        static Dictionary<Editor, PrefabAssetTarget> prefabTargets = new Dictionary<Editor, PrefabAssetTarget>();*/

        /*
        [InitializeOnInspector]
        static void OnInspector()
        {
            Runtime.SmartInspector.OnSetupEditorElement += x =>
            {
                if (x.header.name == "Prefab ImporterHeader")
                {
                    // Hide vanilla prefab header, so we can make our own
                    x.header.style.display = DisplayStyle.None;
                    x.inspector.style.display = DisplayStyle.None;
                }
            };
        }*/
        
        protected override IEnumerable<Patch> GetPatches()
        {
            var importerType = EditorAssembly.GetType("UnityEditor.PrefabImporterEditor");

            assetTargetInfo = AccessTools.Property(importerType, "assetTarget");
            cacheHasMixedBaseInfo = AccessTools.Method(importerType, "CacheHasMixedBaseVariants");
            openPrefab = AccessTools.Method(typeof(PrefabStageUtility), "OpenPrefab", new [] { typeof(string) });
            
            yield break;
/*
            var onEnable = AccessTools.Method(importerType, "OnEnable");
            var onInspectorGUI = AccessTools.Method(importerType, "OnInspectorGUI");
            var onHeaderControlsGUI = AccessTools.Method(importerType, "OnHeaderControlsGUI");

            yield return new Patch(onEnable, postfix: nameof(OnEnable_));
            yield return new Patch(onInspectorGUI, nameof(_OnInspectorGUI), apply: Apply.OnGUI);
            yield return new Patch(onHeaderControlsGUI, nameof(_OnHeaderControlsGUI), nameof(OnHeaderControlsGUI_), apply: Apply.OnGUI);*/
        }

        /*
        static void OnEnable_(Editor __instance)
        {
            var assetTarget = (Object)assetTargetInfo.GetValue(__instance);
                
            if (assetTarget is DefaultAsset)
                return;
            
            var variantBase = PrefabUtility.GetCorrespondingObjectFromSource(assetTarget);
            
            if (!prefabTargets.ContainsKey(__instance))
                prefabTargets.Add(__instance, null);

            prefabTargets[__instance] = new PrefabAssetTarget
            {
                assetTarget = assetTarget,
                variantBase = variantBase,
            };
        }
        
        static bool _OnInspectorGUI(List<string> ___m_PrefabsWithMissingScript)
        {
            // Draw vanilla GUI for missing components handling
            if (___m_PrefabsWithMissingScript.Count > 0)
                return true;
            
            return false;
        }
        
        static bool _OnHeaderControlsGUI(Editor __instance, int ___m_HasMixedBaseVariants, List<string> ___m_PrefabsWithMissingScript)
        {
            if (___m_PrefabsWithMissingScript.Count > 0)
                return true;
            
            if (!prefabTargets.TryGetValue(__instance, out var target))
                return false;

            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            
            if (target.variantBase != null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    if (___m_HasMixedBaseVariants == -1)
                        cacheHasMixedBaseInfo.Invoke(__instance, null);

                    var labelRect = GUILayoutUtility.GetRect(50, EditorGUIUtility.singleLineHeight);

                    tempContent.text = "Base";
                    EditorGUI.PrefixLabel(labelRect, tempContent);
                    
                    EditorGUI.showMixedValue = ___m_HasMixedBaseVariants == 1;
                    EditorGUILayout.ObjectField(target.variantBase, typeof(GameObject), false);
                    EditorGUI.showMixedValue = false;
                }
            }

            if (GUILayout.Button("Open"))
            {
                var rootPath = AssetDatabase.GetAssetPath(target.assetTarget);
                
                openPrefab.Invoke(null, new [] { rootPath });

                GUIUtility.ExitGUI();
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(-10);
            return false;
        }
        static void OnHeaderControlsGUI_(Editor __instance)
        {
            return;
        }*/
    }
}
