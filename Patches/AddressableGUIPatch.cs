using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEditor;

namespace AV.Inspector
{
    internal class AddressableGUIPatch : PatchBase
    {
        static InspectorPrefs prefs => InspectorPrefs.Loaded;
        static bool showAddressable => !prefs.enablePlugin || prefs.additionalButtons.addressable;
        
        
        protected override IEnumerable<Patch> GetPatches()
        {
            var addressableGUIType = GetAddressableGUIType();
            if (addressableGUIType == null) 
                yield break;
            
            var onPostHeaderGUI = AccessTools.Method(addressableGUIType, "OnPostHeaderGUI");

            yield return new Patch(onPostHeaderGUI, nameof(OnAddressablePostHeaderGUI));
        }
        
        static Type GetAddressableGUIType()
        {
            return TypeCache
                .GetTypesWithAttribute<InitializeOnLoadAttribute>()
                .FirstOrDefault(type => type.Name == "AddressableAssetInspectorGUI");
        }

        static bool OnAddressablePostHeaderGUI()
        {
            return showAddressable;
        }
    }
}