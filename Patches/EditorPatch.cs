using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal class EditorPatch : PatchBase
    {
        static InspectorPrefs prefs => InspectorPrefs.Loaded;
        
        protected override IEnumerable<Patch> GetPatches()
        {
            //var canBeExpanded = AccessTools.Method(typeof(Editor), "CanBeExpandedViaAFoldoutWithoutUpdate");
            var drawDefaultInspector = typeof(Editor).GetMethod("DoDrawDefaultInspector", BindingFlags.Static | BindingFlags.NonPublic);
            
            //yield return new Patch(canBeExpanded, prefix: nameof(_CanBeExpandedViaAFoldoutWithoutUpdate));
            yield return new Patch(drawDefaultInspector, prefix: nameof(_DoDrawDefaultInspector));
        }

        // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/Editor.cs#L732
        static bool _DoDrawDefaultInspector(ref bool __result, SerializedObject obj)
        {
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();

            var showScriptField = prefs.showScriptField;
            
            var property = obj.GetIterator();
            var expanded = true;
            var wasScriptFound = false;
            
            while (property.NextVisible(expanded))
            {
                if (!wasScriptFound)
                {
                    if (property.propertyPath == "m_Script")
                    {
                        wasScriptFound = true;
                        
                        if (showScriptField)
                            using (new EditorGUI.DisabledScope(true))
                                EditorGUILayout.PropertyField(property);
                            
                        continue;
                    }
                }

                EditorGUILayout.PropertyField(property, true);
                expanded = false;
            }

            obj.ApplyModifiedProperties();
            __result = EditorGUI.EndChangeCheck();
            
            return false;
        }
        
        
        // TODO: This causes a zero-padding issue on Sorting Group component, and most likely on some other ones..
        /*
        // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/Editor.cs#L1161
        static bool _CanBeExpandedViaAFoldoutWithoutUpdate(ref bool __result, SerializedObject ___m_SerializedObject)
        {
            if (___m_SerializedObject == null)
                return true;
            
            var property = ___m_SerializedObject.GetIterator(); // This eats only about 0.45ms ~
            __result = property.hasVisibleChildren;
            
            // Skip it. This method can eat 7ms with just 24 calls, and it is called during OnGUI (2 times! In EditorGUI and EditorElement).
            // It goes through every child property and calculates their heights.
            // I think it can safely be replaced with property.hasVisibleChildren, or skipped entirely!
            return false;
        }*/
    }
}