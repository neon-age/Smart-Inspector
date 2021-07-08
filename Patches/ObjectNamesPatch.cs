using System.Collections.Generic;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal class ObjectNamesPatch : PatchBase
    {
        static InspectorPrefs prefs = InspectorPrefs.Loaded;
        
        protected override IEnumerable<Patch> GetPatches()
        {
            yield return new Patch(AccessTools.Method(typeof(ObjectNames), "GetInspectorTitle"), postfix: nameof(GetInspectorTitle_));
        }

        static void GetInspectorTitle_(ref string __result)
        {
            if (prefs.showScript)
                return;

            if (__result.EndsWith(" (Script)"))
                __result = __result.Remove(__result.Length - 9, 9);
        }
    }
}