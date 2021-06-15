using System.Collections.Generic;
using HarmonyLib;
using UnityEditor;

namespace AV.Inspector
{
    internal class EditorPatch : PatchBase
    {
        protected override IEnumerable<Patch> GetPatches()
        {
            var canBeExpanded = AccessTools.Method(typeof(Editor), "CanBeExpandedViaAFoldoutWithoutUpdate");
            
            yield return new Patch(canBeExpanded, prefix: nameof(_CanBeExpandedViaAFoldoutWithoutUpdate));
        }

        // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/Editor.cs#L1161
        static bool _CanBeExpandedViaAFoldoutWithoutUpdate(ref bool __result, SerializedObject ___m_SerializedObject)
        {
            if (___m_SerializedObject == null)
                return true;
            
            var property = ___m_SerializedObject.GetIterator(); // This eats only about 0.45ms ~
            __result = property.hasVisibleChildren;
            
            // Skip it. This method can eat 7ms with just 24 calls, and it is called during OnGUI (2 times! In EditorGUI and EditorElement).
            // It goes throw every child property and calculates their heights.
            // I think it can safely be replaced with property.hasVisibleChildren
            return false;
        }
    }
}