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
            var onEnable = AccessTools.Method(PropertyEditorRef.type, "OnEnable");
            var onDisable = AccessTools.Method(PropertyEditorRef.type, "OnDisable");
            
            var rebuildContent = AccessTools.Method(PropertyEditorRef.type, "RebuildContentsContainers");
            var endRebuildContent = AccessTools.Method(PropertyEditorRef.type, "EndRebuildContentContainers");

            
            yield return new Patch(onEnable, postfix: nameof(OnEnable_));
            yield return new Patch(onDisable, nameof(_OnDisable));
            
            yield return new Patch(rebuildContent, nameof(_RebuildContentsContainers), nameof(RebuildContentsContainers_));
            #if UNITY_2020_1_OR_NEWER
            yield return new Patch(endRebuildContent, postfix: nameof(EndRebuildContentContainers_));
            #endif
        }

        static void OnEnable_(EditorWindow __instance)
        {
            InitInspector(__instance);
        }

        static SmartInspector InitInspector(EditorWindow window)
        {
            var inspector = new SmartInspector(window);
            SmartInspector.Injected.AddOrAssign(window, inspector);
            
            inspector.OnEnable();
            return inspector;
        }
        
        static void _OnDisable(EditorWindow __instance)
        {
            if (SmartInspector.Injected.TryGetValue(__instance, out var inspector))
            {
                inspector.OnDisable();
                SmartInspector.Injected.Remove(__instance);
            }
        }

        
        static void _RebuildContentsContainers(EditorWindow __instance)
        {
            if (!SmartInspector.Injected.TryGetValue(__instance, out var inspector))
            {
                inspector = InitInspector(__instance);
            }

            SmartInspector.RebuildingInspector = inspector;
            inspector.OnRebuildContent(SmartInspector.RebuildStage.BeforeEditorElements);
        }
        
        static void RebuildContentsContainers_(EditorWindow __instance)
        {
            if (!SmartInspector.Injected.TryGetValue(__instance, out var inspector))
                return;

            inspector.OnRebuildContent(SmartInspector.RebuildStage.AfterRepaint);

#if !UNITY_2020_1_OR_NEWER
            EndRebuildContentContainers_(__instance);
            #endif
        }
        
        
        static void EndRebuildContentContainers_(EditorWindow __instance)
        {
            if (!SmartInspector.Injected.TryGetValue(__instance, out var inspector))
                return;
            
            // Inject only after all EditorElements are present
            inspector.Inject();
            
            inspector.OnRebuildContent(SmartInspector.RebuildStage.BeforeRepaint);
        }
    }
}