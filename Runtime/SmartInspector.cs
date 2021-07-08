using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public delegate void OnSetupElement(EditorElement x);
        
        public static OnSetupElement OnSetupEditorElement;
    }
}
