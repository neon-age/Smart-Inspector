
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AV.Inspector
{
    internal class UIResources : ScriptableObject
    {
        public static UIResources Asset
        {
            get
            {
                var asset = Index.Asset.UIResources;
                asset.FixUIToolkitReferences();
                return asset;
            }
        }
        
        [Header("Style Sheets")]
        public StyleSheet commonStyles = default;
        public StyleSheet tooltipStyle = default;
        public StyleSheet scrollViewStyle = default;
        public StyleSheet componentsHeaderStyle = default;
        public StyleSheet componentsToolbarStyle = default;
        public StyleSheet componentsToolbarLightStyle = default;
        public StyleSheet inspectorMainToolbarStyle = default;
        
        [Header("Icons")]
        public Texture2D pinIcon = default;
        public Texture2D debugIcon = default;
        
        bool fixedUIToolkitReferences;
        
        static MethodInfo GetFieldInfoFromPropertyPath;

        
        void FixUIToolkitReferences()
        {
            if (fixedUIToolkitReferences)
                return;
            
            var serializedObject = new SerializedObject(this);
            var iterator = serializedObject.GetIterator();
            
            while (iterator.NextVisible(true))
            {
                var path = iterator.propertyPath;
                
                if (path == "m_Script")
                    continue;

                if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                    continue;

                // objectReferenceValue doesn't return null on (uninitialized?) UIE assets!
                var reference = iterator.objectReferenceValue;

                if (reference == null)
                    //Debug.Log($"{path} is null");
                    continue;

                var fieldInfo = GetFieldInfoFromProperty(typeof(UIResources), iterator, out var fieldType);
                
                reference = (Object)fieldInfo.GetValue(this);

                // (reference is StyleSheet style && style == null) returns true on referenced uninitialized asset..
                if (!IsUIEAssetAndNull(reference))
                    continue;

                var assetPath = AssetDatabase.GetAssetPath(reference);
                var loadedAsset = AssetDatabase.LoadAssetAtPath(assetPath, fieldType);

                if (loadedAsset == null)
                    //Debug.Log($"{assetPath} {fieldType?.Name} loaded asset is null");
                    continue;
                
                fieldInfo.SetValue(this, loadedAsset);
                
                fixedUIToolkitReferences = true;
            }
        }

        static bool IsUIEAssetAndNull(Object asset)
        {
            return asset is StyleSheet style && style == null ||
                   asset is VisualTreeAsset tree && tree == null;
        }

        static FieldInfo GetFieldInfoFromProperty(Type host, SerializedProperty property, out Type fieldType)
        {
            if (GetFieldInfoFromPropertyPath == null)
            {
                var utilType = typeof(Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                GetFieldInfoFromPropertyPath = utilType.GetMethod("GetFieldInfoFromPropertyPath", BindingFlags.NonPublic | BindingFlags.Static);
            }
            
            var args = new object[] { host, property.propertyPath, default(Type) };
            var fieldInfo = (FieldInfo)GetFieldInfoFromPropertyPath.Invoke(null, args);
            fieldType = (Type)args[2];
            
            return fieldInfo;
        }
    }
}