using System.Collections.Generic;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal class PropertyEditorPatch : PatchBase
    {
        protected override IEnumerable<Patch> GetPatches()
        {
            var rebuildContent = AccessTools.Method(PropertyEditorRef.type, "RebuildContentsContainers");
            var endRebuildContent = AccessTools.Method(PropertyEditorRef.type, "EndRebuildContentContainers");

            yield return new Patch(rebuildContent, postfix: nameof(RebuildContentsContainers_));
            #if UNITY_2020_1_OR_NEWER
            yield return new Patch(endRebuildContent, postfix: nameof(EndRebuildContentContainers_));
            #endif
        }

        static void RebuildContentsContainers_(EditorWindow __instance)
        {
            InspectorInjection.onInspectorRebuild?.Invoke(__instance, InspectorInjection.RebuildStage.PostfixAfterRepaint);
            #if !UNITY_2020_1_OR_NEWER
            EndRebuildContentContainers_(__instance);
            #endif
        }
        
        static void EndRebuildContentContainers_(EditorWindow __instance)
        {
            InspectorInjection.onInspectorRebuild?.Invoke(__instance, InspectorInjection.RebuildStage.EndBeforeRepaint);
        }
    }
}