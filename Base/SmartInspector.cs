using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using AV.Inspector.Runtime;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using EditorElement = AV.Inspector.Runtime.SmartInspector.EditorElement;

namespace AV.Inspector
{
    internal partial class SmartInspector
    {
        const int ComponentPaddingBottom = 5;
        
        static readonly TypeCache.MethodCollection InitOnInspectorMethods = TypeCache.GetMethodsWithAttribute<InitializeOnInspectorAttribute>();

        internal enum InjectResult
        {
            NoList,
            NoGameObjectEditor,
            Final
        }

        internal enum RebuildStage
        {
            BeforeEditorElements,
            AfterRepaint,
            BeforeRepaint
        }

        internal static SmartInspector LastActive;
        internal static SmartInspector RebuildingInspector;
        internal static Dictionary<EditorWindow, SmartInspector> Injected = new Dictionary<EditorWindow, SmartInspector>();

        
        internal EditorWindow propertyEditor;
        
        VisualElement root;
        VisualElement mainContainer;
        VisualElement contentViewport;
        VisualElement editorsList;
        VisualElement gameObjectEditor;
        
        internal InspectorComponentsToolbar toolbar;
        internal TooltipElement tooltip { get; private set; }

        internal static bool wasAnyLoaded;
        internal readonly Dictionary<VisualElement, EditorElement> editors = new Dictionary<VisualElement, EditorElement>();

        
        [InitializeOnLoadMethod]
        static void OnLoad() => wasAnyLoaded = false;
        
        
        internal SmartInspector(EditorWindow propertyEditor)
        {
            this.propertyEditor = propertyEditor;
        }

        internal void OnEnable() {}
        internal void OnDisable() {}
        
        internal void Inject()
        {
            var result = TryInject();
            //Debug.Log(result);
        }
        InjectResult TryInject()
        {
            LastActive = this;
            
            this.root = propertyEditor.rootVisualElement;
            this.mainContainer = root.Query(className: "unity-inspector-main-container").First();
            this.editorsList = root.Query(className: "unity-inspector-editors-list").First();
            
            if (editorsList == null)
                return InjectResult.NoList;

            Init();

            var scrollView = root.Query(className: "unity-inspector-root-scrollview").First();
            
            contentViewport = scrollView.Get("unity-content-viewport");
            contentViewport.RegisterCallback<GeometryChangedEvent>(OnContentViewportLayout);

            if (gameObjectEditor == null)
                return InjectResult.NoGameObjectEditor;
            
            toolbar = editorsList.Query<InspectorComponentsToolbar>();
            
            if (toolbar == null)
                toolbar = new InspectorComponentsToolbar();
            
            // Insert after InspectorElement and before Footer
            gameObjectEditor.Insert(2, toolbar);
            
            
            return InjectResult.Final;
        }
        
        void Init()
        {
            tooltip = root.Get<TooltipElement>(".smart-inspector--tooltip");
            
            if (tooltip == null)
            {
                tooltip = new TooltipElement();
                tooltip.AddToClassList("smart-inspector--tooltip");
                root.Add(tooltip);
            }

            root.styleSheets.Add(mainStyleSheet);
            
            root.styleSheets.Add(UIResources.Asset.scrollViewStyle);
            editorsList.styleSheets.Add(UIResources.Asset.componentsHeaderStyle);
        }

        void OnContentViewportLayout(GeometryChangedEvent evt)
        {
            // Hides scrollbar indentation
            contentViewport.style.marginRight = 0;
        }

        
        void FirstInitOnInspectorIfNeeded()
        {
            if (wasAnyLoaded) 
                return;
            
            foreach (var initOnInspector in InitOnInspectorMethods)
                try
                {
                    initOnInspector.Invoke(null, null);
                }
                catch (Exception ex) { Debug.LogException(ex); }
            
            wasAnyLoaded = true;
        }

        /// <see cref="PropertyEditorPatch.RebuildContentsContainers_"/>
        internal void OnRebuildContent(RebuildStage stage)
        {
            RebuildingInspector = this;
            FirstInitOnInspectorIfNeeded();

            //Debug.Log(stage);
            
            if (stage == RebuildStage.BeforeEditorElements)
            {
                editors.Clear();
                RemoveUserElements();
            }
            
            if (stage == RebuildStage.BeforeRepaint)
            {
                RebuildToolbar();
                FixComponentLayout();
            }
            
            if (stage == RebuildStage.AfterRepaint)
            {
            }
        }

        void RebuildToolbar()
        {
            toolbar?.Rebuild(this);
        }

        void RemoveUserElements()
        {
            editorsList?.Query(className: Runtime.SmartInspector.UserElementClass).ForEach(x =>
            {
                //Debug.Log(x);
                x.RemoveFromHierarchy();
            });
        }
        void FixComponentLayout()
        {
            editorsList?.Query(className: "component").ForEach(x =>
            {
                if (x.resolvedStyle.fontSize == 0) // Not resolved yet
                    return;
                
                // Is there be a proper way to do that?
                // This magically triggers immediate layout repaint that we need..
                // Fixes an issue when collapsed components kept expanded layout (no idea why!)
                x.style.fontSize = 12 + (Random.value / 10000);
            });
        }

        /// <see cref="EditorElementPatch.Init_"/>
        public void SetupEditorElement(EditorElement x)
        {
            var element = x.element;
            var header = x.header;
            var inspector = x.inspector;
            var footer = x.footer;
            
            if (x.isGo)
            {
                // Tabs bar is injected inside GO editor
                gameObjectEditor = element;
            }
            
            element.AddClass("editor-element");
            header.AddClass("header");
            inspector.AddClass("inspector");
            footer.AddClass("footer");
            
            
            SetupEditorElement();
            SetupHeader();
            SetupInspectorElement();
            // Footer is manipulated by EditorElementPatch.Init_
            
            editors.AddOrAssign(element, x);
            //Debug.Log($"setup {x.name}");
            Runtime.SmartInspector.OnSetupEditorElement?.Invoke(x);


            void SetupEditorElement()
            {
                element.EnableClass("game-object", x.isGo);
                element.EnableClass("transform", x.isTransform);
                element.EnableClass("component", x.isComponent);
                element.EnableClass("material", x.isMaterial);
                
                #if !UNITY_2020_1_OR_NEWER
                if (x.isTransform)
                    element.style.top = -2;
                #endif
            }
            void SetupHeader()
            {
            }
            void SetupInspectorElement()
            {
                inspector.Register<GeometryChangedEvent>(evt =>
                {
                    if (element == null)
                        return;
                    var isExpanded = InternalEditorUtility.GetIsInspectorExpanded(x.target);
                    element.EnableClass("is-expanded", isExpanded);
                });

                if (x.isComponent) 
                    SetupComponentInspector();
            }

            void SetupComponentInspector()
            {
                var inspectorContainer = inspector.Get<IMGUIContainer>() ?? 
                                         inspector.Get(".unity-inspector-element__custom-inspector-container");

                if (inspectorContainer != null)
                {
                    // Dragging is calculated based on container layout (it ignores InspectorElement padding)
                    inspectorContainer.style.paddingBottom = ComponentPaddingBottom;
                    inspector.style.paddingBottom = 0;
                }
            }
        }

        public static string GetInspectorTitle(Object obj)
        {
            return ObjectNames.GetInspectorTitle(obj).Replace("(Script)", "");
        }
    }
}