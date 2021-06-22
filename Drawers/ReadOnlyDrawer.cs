using System;
using System.Collections;
using System.Collections.Generic;
using AV.Inspector.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    // TODO: DecoratorDrawer UITK support
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            EditorGUI.BeginDisabledGroup(true);
        }
        
        // This causes stack overflow and crash with default UITK Inspector. LOL. Great job Unity!
        // https://docs.unity3d.com/ScriptReference/PropertyDrawer.html
        // TODO: Now I have to make a patch / utility that gets me being-iterated element...
        /*
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new PropertyField(property);
            return field;
        }*/
    }
}
