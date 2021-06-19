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
    internal class InspectorAssetPreviewPatch : PatchBase
    {
        protected override IEnumerable<Patch> GetPatches()
        {
            var addressableGUIType = GetAddressableGUIType();
            if (addressableGUIType != null)
            {
                var onPostHeaderGUI = AccessTools.Method(addressableGUIType, "OnPostHeaderGUI");

                yield return new Patch(onPostHeaderGUI, nameof(OnPostHeaderGUI));
            }
        
            var labelGUIType = EditorAssembly.GetType("UnityEditor.LabelGUI");
            var assetBundleGUIType = EditorAssembly.GetType("UnityEditor.AssetBundleNameGUI");

            var onLabelGUI = AccessTools.Method(labelGUIType, "OnLabelGUI");
            var onAssetBundleGUI = AccessTools.Method(assetBundleGUIType, "OnAssetBundleNameGUI");
            var drawPreviewAndLabels = AccessTools.Method(PropertyEditorRef.type, "DrawPreviewAndLabels");

            yield return new Patch(onLabelGUI, nameof(_OnLabelGUI));
            yield return new Patch(onAssetBundleGUI, nameof(_OnAssetBundleNameGUI), apply: Apply.OnGUI);
            yield return new Patch(drawPreviewAndLabels, nameof(DrawPreviewAndLabels_), apply: Apply.OnGUI);
        }
        
        static Type GetAddressableGUIType()
        {
            return TypeCache
                .GetTypesWithAttribute<InitializeOnLoadAttribute>()
                .FirstOrDefault(type => type.Name == "AddressableAssetInspectorGUI");
        }
        
        static bool OnPostHeaderGUI()
        {
            return false;
        }

        static bool _OnLabelGUI()
        {
            return false;
        }

        static bool _OnAssetBundleNameGUI()
        {
            return false;
        }
        
        static void DrawPreviewAndLabels_(EditorWindow __instance, bool ___m_HasPreview, bool ___m_PreviousPreviewExpandedState)
        {
            var footerInfo = __instance.rootVisualElement.Query(className: "unity-inspector-footer-info").First();
            if (footerInfo == null)
                return;

            footerInfo.visible = ___m_HasPreview;

            if (___m_PreviousPreviewExpandedState)
                footerInfo.style.marginBottom = -6;
            else
                footerInfo.style.marginBottom = 0;
        }
    }
}
