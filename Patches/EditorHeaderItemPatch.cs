using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal class EditorHeaderItemPatch : PatchBase
    {
        static InspectorPrefs prefs => InspectorPrefs.Loaded;
        static bool showHelp => !prefs.enablePlugin || prefs.headers.showHelp;
        static bool showPreset => !prefs.enablePlugin || prefs.headers.showPresets;
        static bool showMenu => !prefs.enablePlugin || prefs.headers.showMenu;
        
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
            return showHelp;
        }

        static bool _DrawPresetButton()
        {
            return showPreset;
        }
    }
}