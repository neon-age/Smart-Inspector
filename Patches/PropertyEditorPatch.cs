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
            yield return new Patch(endRebuildContent, postfix: nameof(EndRebuildContentContainers_));
        }

        static void RebuildContentsContainers_(EditorWindow __instance)
        {
            InspectorInjection.onInspectorRebuild?.Invoke(__instance, InspectorInjection.RebuildStage.PostfixAfterRepaint);
        }
        
        static void EndRebuildContentContainers_(EditorWindow __instance)
        {
            InspectorInjection.onInspectorRebuild?.Invoke(__instance, InspectorInjection.RebuildStage.EndBeforeRepaint);
        }
    }
}