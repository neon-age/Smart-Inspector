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
using Space = AV.Inspector.Runtime.SmartInspector.Space;

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

        internal class SubData : VisualElement
        {
            public SmartInspector smartInspector;
            public EditorElement element;
        }

        internal static SmartInspector LastActive;
        internal static SmartInspector RebuildingInspector;
        internal static Dictionary<EditorWindow, SmartInspector> Injected = new Dictionary<EditorWindow, SmartInspector>();

        
        internal EditorWindow propertyEditor;
        internal ActiveEditorTracker tracker;
        
        VisualElement root;
        VisualElement mainContainer;
        VisualElement contentViewport;
        VisualElement editorsList;
        VisualElement gameObjectEditor;
        
        internal InspectorTabsBar toolbar;
        internal TooltipElement tooltip { get; private set; }

        internal static bool wasAnyLoaded;
        internal readonly Dictionary<VisualElement, EditorElement> editors = new Dictionary<VisualElement, EditorElement>();
        
        [InitializeOnLoadMethod]
        static void OnLoad() => wasAnyLoaded = false;
        
        
        internal SmartInspector(EditorWindow propertyEditor)
        {
            this.propertyEditor = propertyEditor;
            this.tracker = PropertyEditorRef.GetTracker(propertyEditor);
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
            
            contentViewport = scrollView.Query(name: "unity-content-viewport");
            contentViewport.RegisterCallback<GeometryChangedEvent>(OnContentViewportLayout);

            if (gameObjectEditor == null)
                return InjectResult.NoGameObjectEditor;
            
            toolbar = editorsList.Query<InspectorTabsBar>();
            
            if (toolbar == null)
                toolbar = new InspectorTabsBar();
            
            // Insert after InspectorElement and before Footer
            gameObjectEditor.Insert(2, toolbar);
            
            
            return InjectResult.Final;
        }
        
        void Init()
        {
            tooltip = root.Query<TooltipElement>(className: "smart-inspector--tooltip");
            
            if (tooltip == null)
            {
                tooltip = new TooltipElement();
                tooltip.AddToClassList("smart-inspector--tooltip");
                root.Add(tooltip);
            }

            root.styleSheets.Add(coreStyles);
            
            if (!EditorGUIUtility.isProSkin)
                root.styleSheets.Add(coreStylesLight);
            
            root.styleSheets.Add(scrollViewStyles);
            editorsList.styleSheets.Add(headerStyles);
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
            }
            
            if (stage == RebuildStage.AfterRepaint)
            {
                // TODO: Fix one-frame issue when dragged element keeps expanded layout of element that was last there
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

        
        /// <see cref="EditorElementPatch.Init_"/>
        public void SetupEditorElement(EditorElement x)
        {
            var element = x.element;
            var header = x.header;
            var inspector = x.inspector;
            var footer = x.footer;
            var data = x.Get<SubData>();
            
            if (x.isGo)
            {
                // Tabs bar is injected inside GO editor
                gameObjectEditor = element;
            }
            
            element.AddClass("editor-element");
            header.AddClass("header");
            inspector.AddClass("inspector");
            footer.AddClass("footer");

            SetupSubData();
            SetupEditorElement();
            SetupHeader();
            SetupInspectorElement();
            // Footer is manipulated by EditorElementPatch.Init_
            
            editors.AddOrAssign(element, x);
            try
            {
                Runtime.SmartInspector.OnSetupEditorElement?.Invoke(x);
            }
            catch (Exception ex) { Debug.LogException(ex); }
            

            void SetupSubData()
            {
                if (data == null)
                    inspector.Add(data = new SubData());
                
                data.smartInspector = this;
                data.element = x;
            }
            
            void SetupEditorElement()
            {
                x.Register<GeometryChangedEvent>(evt => OnElementLayout(data));
                
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
                if (x.isComponent)
                {
                    x.header.FlexDirection(FlexDirection.RowReverse);
                    
                    // Temp solution for component header buttons padding
                    if (!x.header.Has<Space>("#rightSpace"))
                    {
                        x.header.x.Add(new Space(64) { name = "rightSpace" });
                    }
                }
            }
            void SetupInspectorElement()
            {
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

        void OnElementLayout(SubData data)
        {
            var x = data.element;

            if (x.isComponent) OnComponentLayout(x);
            if (x.isMaterial) OnMaterialLayout(x);
        }

        void OnComponentLayout(EditorElement x)
        {
            // Do this only once on selection change for components
            if (x.expandedState != -1)
                return;
            
            x.expandedState = InternalEditorUtility.GetIsInspectorExpanded(x.target) ? 1 : 0;

            SetElementVisible(x, x.isExpanded);
        }

        void OnMaterialLayout(EditorElement x)
        {
            var isExpanded = InternalEditorUtility.GetIsInspectorExpanded(x.target);

            if (x.isExpanded != isExpanded)
            {
                SetElementVisible(x, isExpanded, changeDisplay: false);
                
                // Material doesn't care about our flexing, unless we set global expand state
                if (x.isMaterial)
                    InternalEditorUtility.SetIsInspectorExpanded(x.target, x.isExpanded);
            }
        }
        
        
        void SetElementVisible(EditorElement x, bool visible, bool changeDisplay = true)
        {
            x.expandedState = visible ? 1 : 0;
            x.element.EnableClass("is-expanded", visible);

            if (changeDisplay)
                x.inspector.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            
            //Debug.Log($"{x.name} {visible}");
        }
        

        public static string GetInspectorTitle(Object obj)
        {
            return ObjectNames.GetInspectorTitle(obj).Replace("(Script)", "");
        }
    }
}