using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace AV.Inspector
{
    internal class InspectorUtilsPatch : PatchBase
    {
        protected override IEnumerable<Patch> GetPatches()
        {
            var type = EditorAssembly.GetType("UnityEditor.InspectorWindowUtils");
            yield return new Patch(AccessTools.Method(type, "DrawAddedComponentBackground"), nameof(_DrawAddedComponentBackground));
        }

        static bool _DrawAddedComponentBackground()
        {
            return false;
        }
    }
}