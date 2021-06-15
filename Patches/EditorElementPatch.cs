using System;
using System.Collections;
using System.Collections.Generic;
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

namespace AV.Inspector
{
    internal class EditorElementPatch : PatchBase
    {
        const int ComponentPaddingBottom = 5;
        
        const int LongHoldMS = 600;
        const int SlowMouseMoveDelayMS = 400;
        const float SlowMouseDeltaThreshold = 40;
        
        const float DragAreaHeight = 22;
        const float DragMarkerHeight = 4;
        const float DragMarkerY = 2;
        
        static readonly Type EditorDraggingType = EditorAssembly.GetType("UnityEditor.EditorDragging");
        static readonly MethodInfo get_m_InspectorElement = AccessTools.PropertyGetter(EditorElementRef.type, "m_InspectorElement");
    
        static Action onPerformDrag;
        static bool IsAnyDragging;
        
        static Vector2 mousePos;
        static Vector2 holdMousePos;
        

        protected override IEnumerable<Patch> GetPatches()
        {
            yield return new Patch(AccessTools.Method(EditorDraggingType, "GetTargetRect"), postfix: nameof(GetTargetRect_));
            yield return new Patch(AccessTools.Method(EditorDraggingType, "GetMarkerRect"), postfix: nameof(GetMarkerRect_));
            yield return new Patch(AccessTools.Method(EditorDraggingType, "HandleDragPerformEvent"), prefix: nameof(_HandleDragPerformEvent));
            
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "Init"), postfix: nameof(Init_));
            yield return new Patch(AccessTools.Method(EditorElementRef.type, "UpdateInspectorVisibility"), prefix: nameof(_UpdateInspectorVisibility));
        }

        static void _HandleDragPerformEvent(object ___m_InspectorWindow, Editor[] editors, Event evt, ref int targetIndex)
        {
            onPerformDrag?.Invoke();
            onPerformDrag = null;
            IsAnyDragging = false;
            
            var window = (EditorWindow)___m_InspectorWindow;
            InspectorInjection.TryGetInspector(window, out var inspector);
        }

        static bool _UpdateInspectorVisibility(VisualElement __instance)
        {
            // Skip default padding
            return false;
        }

        static void DeconstructEditorElement(VisualElement element, out Editor editor, out int editorIndex, out Object target)
        {
            editor = EditorElementRef.GetEditor(element);
            editorIndex = EditorElementRef.GetEditorIndex(element);
            target = editor.target;
        }

        static void Init_(VisualElement __instance, IMGUIContainer ___m_Header, IMGUIContainer ___m_Footer, object ___inspectorWindow)
        {
            ___m_Header.AddToClassList("header");
            ___m_Footer.AddToClassList("footer");
            __instance.AddToClassList("editor-element");
            
            var dragging = false;
            var window = (EditorWindow)___inspectorWindow;
            
            var editorsList = (VisualElement)null;
            var inspector = (VisualElement)get_m_InspectorElement.Invoke(__instance, null);

            DeconstructEditorElement(__instance, out var editor, out var index, out var target);
            //var tracker = PropertyEditorRef.GetTracker(window);

            if (target is Component)
            {
                var imgui = inspector.Query<IMGUIContainer>().First();
                
                // Dragging is calculated based on IMGUI layout
                imgui.style.paddingBottom = ComponentPaddingBottom;
                inspector.style.paddingBottom = 0;
            }
            
            __instance.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);

            void OnAttachToPanel(AttachToPanelEvent _)
            {
                editorsList = __instance.parent;
            }
            
            // Footer == Drag/Drop Area
            ___m_Footer.visible = false;
            ___m_Footer.pickingMode = PickingMode.Ignore;
            ___m_Footer.style.position = Position.Absolute;
            ___m_Footer.style.left = 0;
            ___m_Footer.style.right = 0;
            ___m_Footer.style.bottom = 0;
            ___m_Footer.style.height = DragAreaHeight;
            ___m_Footer.onGUIHandler += () =>
            {
                if (!IsAnyDragging)
                    return;
                
                if (dragging)
                    mousePos = Event.current.mousePosition;
            };
            
            __instance.RegisterCallback<DragEnterEvent>(evt => EnterDrag());
            
            __instance.RegisterCallback<MouseLeaveEvent>(evt => LeaveDrag());
            __instance.RegisterCallback<DragLeaveEvent>(evt => LeaveDrag());
            __instance.RegisterCallback<DragPerformEvent>(evt => LeaveDrag());

            // Hides drag area when user is holding mouse still to drag into object field or expand header.
            async void LeaveOnLongHold()
            {
                // Don't leave until mouse is at rest! Ignore small delta
                while ((holdMousePos - mousePos).magnitude > SlowMouseDeltaThreshold)
                {
                    holdMousePos = mousePos;
                    
                    await Task.Delay(SlowMouseMoveDelayMS);
                    
                    window.Repaint();
                }

                await Task.Delay(LongHoldMS);

                if (dragging)
                    LeaveDrag();

                window.Repaint();
            }


            void OnBeginDrag()
            {
            }
            void OnPerformDrag()
            {
                if (IsAnyDragging)
                {
                    IsAnyDragging = false;
                }
                LeaveDrag();
                
                ___m_Footer.pickingMode = PickingMode.Ignore;
            }
            
            void EnterDrag()
            {
                dragging = true;
                
                if (!IsAnyDragging)
                {
                    IsAnyDragging = true;
                    OnBeginDrag();
                }

                onPerformDrag += OnPerformDrag;
                
                LeaveOnLongHold();
                
                ___m_Footer.visible = true;
                ___m_Footer.pickingMode = PickingMode.Position;
            }
            void LeaveDrag()
            {
                dragging = false;
                ___m_Footer.visible = false;
            }
        }

        static void GetTargetRect_(ref Rect __result, Rect contentRect)
        {
            __result.height = DragAreaHeight;
        }
        
        
        static void GetMarkerRect_(ref Rect __result, Rect ___m_BottomArea, Rect targetRect
        #if !UNITY_2020_3_OR_NEWER
        , float markerY, bool targetAbove
        #endif
        )
        {
            /*
            if (___m_BottomArea.Contains(Event.current.mousePosition))
            {
                __result = targetRect;
                __result.height = DragMarkerHeight;
                __result.y += __result.height;
                return;
            }*/

            __result.y = __result.yMax - DragMarkerHeight + DragMarkerY;
            __result.height = DragMarkerHeight;
        }
    }
}
