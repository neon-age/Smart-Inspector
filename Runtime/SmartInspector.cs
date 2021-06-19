using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public static Action<EditorElement> OnSetupEditorElement;
    }
}
