using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEditor;

namespace AV.Inspector
{
    internal class PostHeaderGUIPatch : PatchBase
    {
        static InspectorPrefs prefs = InspectorPrefs.Loaded;

        static Type addressableType;
        static Type convertToEntityType;
        
        protected override IEnumerable<Patch> GetPatches()
        {
            GetPostHeaderGUITypes();

            if (addressableType != null)
                yield return new Patch(AccessTools.Method(addressableType, "OnPostHeaderGUI"), nameof(_OnAddressableHeader));
            
            if (convertToEntityType != null)
                yield return new Patch(AccessTools.Method(convertToEntityType, "DisplayConvertToEntityHeaderCallBack"), nameof(_OnConvertToEntityHeader));
        }
        
        static void GetPostHeaderGUITypes()
        {
            foreach (var type in TypeCache.GetTypesWithAttribute<InitializeOnLoadAttribute>())
            {
                if (addressableType == null && type.Name == "AddressableAssetInspectorGUI")
                    addressableType = type;
                
                if (convertToEntityType == null && type.Name == "EntityConversionHeader")
                    convertToEntityType = type;
            }
        }

        static bool _OnAddressableHeader()
        {
            return prefs.showAddressable;
        }
        
        static bool _OnConvertToEntityHeader()
        {
            return prefs.showConvertToEntity;
        }
    }
}