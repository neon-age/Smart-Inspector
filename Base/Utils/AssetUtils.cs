
using UnityEditor;
using UnityEngine;

namespace AV.Inspector
{
    internal static class AssetUtils
    {
        public static T LoadFromGUID<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}