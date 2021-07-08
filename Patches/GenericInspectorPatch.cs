using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace AV.Inspector
{
    internal class GenericInspectorPatch : PatchBase
    {
        protected override IEnumerable<Patch> GetPatches()
        {
            var type = EditorAssembly.GetType("UnityEditor.GenericInspector");

            yield return new Patch(AccessTools.Method(type, "GetOptimizedGUIBlock"), nameof(_GetOptimizedGUIBlock));
        }

        // https://github.com/Unity-Technologies/UnityCsReference/blob/2019.4/Editor/Mono/Inspector/GenericInspector.cs#L22
        /// <seealso cref="EditorPatch._DoDrawDefaultInspector"/>
        static bool _GetOptimizedGUIBlock(ref bool __result)
        {
            // This seems to be a nonsense legacy leftover. 
            // I have no idea why it is called "Optimized", but it goes through every property and calculates their heights (2 times).. 
            // We skip it to make sure Editor.DoDrawDefaultInspector is called instead for default GUI.
            // It might also improve performance in cases with lots of user components.
            __result = false;
            return false;
        }
    }
}