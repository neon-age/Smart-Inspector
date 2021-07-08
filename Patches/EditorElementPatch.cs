using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using EditorElement = AV.Inspector.Runtime.SmartInspector.EditorElement;

namespace AV.Inspector
{
    // https://github.com/Unity-Technologies/UnityCsReference/blob/2020.2/Modules/UIElementsEditor/Inspector/EditorElement.cs
    // https://github.com/Unity-Technologies/UnityCsReference/blob/2020.2/Editor/Mono/Inspector/EditorDragging.cs
    internal class EditorElementPatch : PatchBase
    {
        protected override IEnumerable<Patch> GetPatches()
        {
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "Init"), postfix: nameof(Init_));
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "Reinit"), postfix: nameof(Reinit_));
            
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "UpdateInspectorVisibility"), nameof(_UpdateInspectorVisibility));
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "EditorNeedsVerticalOffset"), nameof(_EditorNeedsVerticalOffset));
        }

        
        /// Skip default footer padding (for <see cref="EditorDraggingPatch"/>)
        static bool _UpdateInspectorVisibility(VisualElement __instance)
        {
            return false;
        }
        
        static bool _EditorNeedsVerticalOffset(VisualElement __instance)
        {
            // https://github.com/Unity-Technologies/UnityCsReference/blob/2019.4/Editor/Mono/Inspector/EditorElement.cs#L356
            return false;
        }

        
        static void Init_(VisualElement __instance, IMGUIContainer ___m_Header, IMGUIContainer ___m_Footer, EditorWindow ___inspectorWindow)
        {
            var smartInspector = SmartInspector.RebuildingInspector;
            if (smartInspector == null)
                return;
            
            var data = CreateEditorElement(__instance, ___m_Header, ___m_Footer, smartInspector);
            smartInspector.SetupEditorElement(data);
        }
        
        static void Reinit_(VisualElement __instance, IMGUIContainer ___m_Header, IMGUIContainer ___m_Footer, EditorWindow ___inspectorWindow)
        {
            // TODO: Do proper re-init instead of full recreation 
            Init_(__instance, ___m_Header, ___m_Footer, ___inspectorWindow);
        }
        
        
        internal static EditorElement CreateEditorElement(VisualElement element, IMGUIContainer header, IMGUIContainer footer, SmartInspector smartInspector)
        {
            var editor = EditorElementRef.GetEditor(element);
            var editorIndex = EditorElementRef.GetEditorIndex(element);
            var inspector = EditorElementRef.GetInspectorElement(element);
            
            var window = smartInspector.propertyEditor;
            var tracker = smartInspector.tracker;

            
            var data = new EditorElement(element)
            {
                header = header,
                inspector = inspector,
                footer = footer,
                
                index = editorIndex,
                expandedState = -1,
                
                editor = editor,
                window = window,
                tracker = tracker,
                
                smartInspector = smartInspector,
            };

            return data;
        }
    }
}
