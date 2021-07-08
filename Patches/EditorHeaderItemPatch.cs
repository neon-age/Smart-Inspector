using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal class EditorHeaderItemPatch : PatchBase
    {
        static InspectorPrefs prefs = InspectorPrefs.Loaded;

        protected override IEnumerable<Patch> GetPatches()
        {
            var attributeType = EditorAssembly.GetType("UnityEditor.EditorHeaderItemAttribute");

            foreach (var method in TypeCache.GetMethodsWithAttribute(attributeType))
            {
                var name = method.Name;

                if (name == "HelpIconButton")
                    yield return new Patch(method, nameof(_HelpIconButton), apply: Apply.OnGUI);
                
                if (name == "DrawPresetButton")
                    yield return new Patch(method, nameof(_DrawPresetButton), apply: Apply.OnGUI);
            }
        }

        static bool _HelpIconButton()
        {
            return prefs.showHelp;
        }

        static bool _DrawPresetButton()
        {
            return prefs.showPreset;
        }
    }
}