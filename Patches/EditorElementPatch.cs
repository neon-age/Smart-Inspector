using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
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
        static readonly MethodInfo get_m_InspectorElement = AccessTools.PropertyGetter(EditorElementRef.type, "m_InspectorElement");
    
        static Action onPerformDrag;
        static bool IsAnyDragging;
        
        static Vector2 mousePos;
        static Vector2 holdMousePos;
        

        protected override IEnumerable<Patch> GetPatches()
        {
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "Init"), postfix: nameof(Init_));
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "Reinit"), postfix: nameof(Reinit_));
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "UpdateInspectorVisibility"), prefix: nameof(_UpdateInspectorVisibility));
        }


        /// Skip default footer padding (for <see cref="EditorDraggingPatch"/>)
        static bool _UpdateInspectorVisibility(VisualElement __instance)
        {
            return false;
        }
        
        static void Init_(VisualElement __instance, IMGUIContainer ___m_Header, IMGUIContainer ___m_Footer, EditorWindow ___inspectorWindow)
        {
            var smartInspector = SmartInspector.RebuildingInspector;

            var data = CreateEditorElement(__instance, ___m_Header, ___m_Footer, ___inspectorWindow);
            smartInspector.SetupEditorElement(data);
        }
        static void Reinit_(VisualElement __instance, IMGUIContainer ___m_Header, IMGUIContainer ___m_Footer, EditorWindow ___inspectorWindow)
        {
            var smartInspector = SmartInspector.RebuildingInspector;
            
            var data = CreateEditorElement(__instance, ___m_Header, ___m_Footer, ___inspectorWindow);
            smartInspector.SetupEditorElement(data);
        }
        
        static EditorElement CreateEditorElement(VisualElement element, VisualElement header, VisualElement footer, EditorWindow window)
        {
            var editor = EditorElementRef.GetEditor(element);
            var editorIndex = EditorElementRef.GetEditorIndex(element);
            var inspector = (VisualElement)get_m_InspectorElement.Invoke(element, null);
            var tracker = PropertyEditorRef.GetTracker(window);
            
            var data = new EditorElement(element)
            {
                index = editorIndex,
                header = header,
                inspector = inspector,
                footer = footer,
                editor = editor,
                window = window,
                tracker = tracker,
            };

            element.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);

            void OnAttachToPanel(AttachToPanelEvent _)
            {
                data.list = element.parent;
            }

            return data;
        }
    }
}
