using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace AV.Inspector
{
    internal class AssetPreviewPatch : PatchBase
    {
        static InspectorPrefs prefs = InspectorPrefs.Loaded;

        protected override IEnumerable<Patch> GetPatches()
        {
            var labelGUIType = EditorAssembly.GetType("UnityEditor.LabelGUI");
            var assetBundleGUIType = EditorAssembly.GetType("UnityEditor.AssetBundleNameGUI");

            var onLabelGUI = AccessTools.Method(labelGUIType, "OnLabelGUI");
            var onAssetBundleGUI = AccessTools.Method(assetBundleGUIType, "OnAssetBundleNameGUI");
            var drawPreviewAndLabels = AccessTools.Method(PropertyEditorRef.type, "DrawPreviewAndLabels");

            yield return new Patch(onLabelGUI, nameof(_OnLabelGUI));
            yield return new Patch(onAssetBundleGUI, nameof(_OnAssetBundleNameGUI), apply: Apply.OnGUI);
            yield return new Patch(drawPreviewAndLabels, nameof(DrawPreviewAndLabels_), apply: Apply.OnGUI);
        }

        static bool _OnLabelGUI()
        {
            return prefs.showLabel;
        }

        static bool _OnAssetBundleNameGUI()
        {
            return prefs.showBundle;
        }
        
        static void DrawPreviewAndLabels_(EditorWindow __instance, bool ___m_HasPreview, bool ___m_PreviousPreviewExpandedState)
        {
            var footerInfo = __instance.rootVisualElement.Query(className: "unity-inspector-footer-info").First();
            if (footerInfo == null)
                return;
            
            footerInfo.style.marginBottom = 0;
            footerInfo.visible = true;

            var showLabel = prefs.showLabel;
            var showBundle = prefs.showBundle;

            if (!prefs.enabled) 
                return;
            
            if (!showLabel && !showBundle)
                footerInfo.visible = ___m_HasPreview;

            var expanded = ___m_PreviousPreviewExpandedState;
            if (expanded)
            {
                var margin = 0;
                
                if (!showLabel && !showBundle)
                    margin = -6;
                else if (showLabel && !showBundle)
                    margin = -4;
                else
                    margin = 0;
                
                footerInfo.style.marginBottom = margin;
            }
        }
    }
}
