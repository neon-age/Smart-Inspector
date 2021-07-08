using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using AV.Inspector.Runtime;
using AV.UITK;
using Object = UnityEngine.Object;
using EditorElement = AV.Inspector.Runtime.SmartInspector.EditorElement;
using Space = AV.UITK.FluentUITK.Space;

namespace AV.Inspector
{
    internal partial class SmartInspector
    {
        const string UserElementClass = "user-element";
        const int ComponentPaddingBottom = 5;
        const int SmoothScrollMS = 250;
        const int SmoothScrollMinMS = 10;
        const int SmoothScrollMaxMS = 500;
        
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
        
        static InspectorPrefs prefs = InspectorPrefs.Loaded;

        internal static SmartInspector RebuildingInspector;
        internal static Dictionary<EditorWindow, SmartInspector> Injected = new Dictionary<EditorWindow, SmartInspector>();

        
        internal EditorWindow propertyEditor;
        internal ActiveEditorTracker tracker;
        
        FluentElement<VisualElement> root;
        FluentElement<VisualElement> mainContainer;
        FluentElement<VisualElement> contentViewport;
        FluentElement<ScrollView> scrollView;
        FluentElement<Scroller> scrollbar;
        
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

        internal static void ForEach(Action<SmartInspector> action)
        {
            foreach (var inspector in Injected.Values)
            {
                try { action(inspector); } catch(Exception ex) { Debug.LogException(ex); }
            }
        }
        
        internal void Inject()
        {
            var result = TryInject();
            //Debug.Log(result);
        }
        
        InjectResult TryInject()
        {
            this.root = propertyEditor.rootVisualElement;
            this.mainContainer = root.Get(".unity-inspector-main-container");
            this.editorsList = root.Get(".unity-inspector-editors-list");
            
            if (editorsList == null)
                return InjectResult.NoList;

            scrollView = root.Get<ScrollView>(".unity-inspector-root-scrollview").First();
            scrollbar = scrollView.Get<Scroller>(".unity-scroller--vertical");
            contentViewport = scrollView.Get("#unity-content-viewport");
            
            root.Register<MouseMoveEvent>(OnRootMouseMove);
            contentViewport.Register<GeometryChangedEvent>(OnContentViewportLayout);
            
            SetupScrollView();
            Init();

            if (gameObjectEditor == null)
                return InjectResult.NoGameObjectEditor;
            
            toolbar = editorsList.Query<InspectorTabsBar>();
            
            if (toolbar == null)
                toolbar = new InspectorTabsBar();
            
            // Insert after InspectorElement and before Footer
            gameObjectEditor.Insert(2, toolbar);

            return InjectResult.Final;
        }

        void SetupScrollView()
        {
            scrollView.AddClass("root-scroll");

            foreach (var evt in FluentUITK.GetCallbacks(scrollView))
            {
                if (evt.Is<WheelEvent>())
                    evt.Unregister();
            }
            scrollView.Register<WheelEvent>(OnScrollWheel);
        }

        void OnScrollWheel(WheelEvent evt)
        {
            var layout = scrollView.x.layout;
            var content = scrollView.x.contentContainer;
            var vertical = scrollView.x.verticalScroller;

            if (prefs.useSmoothScrolling)
            {
                var delta = evt.delta.y / 20;

                vertical.experimental.animation.Start(0, delta, SmoothScrollMS, (e, value) =>
                {
                    Scroll(value);
                });
            }
            else
            {
                Scroll(evt.delta.y);
            }

            void Scroll(float delta)
            {
                if (delta != vertical.value)
                    evt.StopPropagation();
                
                if (content.layout.height - layout.height > 0)
                {
                    if (delta < 0)
                        vertical.ScrollPageUp(Mathf.Abs(delta));
                    else if (delta > 0)
                        vertical.ScrollPageDown(Mathf.Abs(delta));
                }
            }
        }

        void Init()
        {
            editorsList.styleSheets.Add(headerStyles);
            
            tooltip = root.Get<TooltipElement>(".smart-inspector--tooltip");
            if (tooltip == null)
            {
                tooltip = new TooltipElement().Fluent().AddClass("smart-inspector--tooltip");
                root.Add(tooltip);
            }
            
            ScrollbarRedraw(Vector2.zero);
        }
        
        void OnUnpatch()
        {
            RemoveUserElements();
        }
        
        void OnRootMouseMove(MouseMoveEvent evt)
        {
            ScrollbarRedraw(evt.mousePosition);
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

            if (root == null)
                return;
            if (!prefs.enabled)
                OnUnpatch();

            DisplayOptionalParts();
            OnContentViewportLayout(null);
            
            //Debug.Log(stage);
            
            if (stage == RebuildStage.BeforeEditorElements)
            {
                editors.Clear();
                RemoveUserElements();
            }
            
            if (stage == RebuildStage.BeforeRepaint)
            {
                RemoveDetachedEditors();
                RebuildToolbar();
            }
            
            if (stage == RebuildStage.AfterRepaint)
            {
                // TODO: Fix one-frame issue when dragged element keeps expanded layout of element that was last there
            }
        }

        void RemoveDetachedEditors()
        {
            var detachedEditors = editors.Where(x => x.Key.parent == null).ToArray();
            foreach (var detached in detachedEditors)
                editors.Remove(detached.Key);
        }
        
        void RebuildToolbar()
        {
            toolbar?.Rebuild(this);
        }

        void RemoveUserElements()
        {
            editorsList?.Query(className: UserElementClass).ForEach(x => x.RemoveFromHierarchy());
        }
        
        
        void OnContentViewportLayout(GeometryChangedEvent evt)
        {
            if (prefs.useCompactScrollbar)
                contentViewport?.Margin(right: 0);
        }
        
        void ScrollbarRedraw(Vector2 mousePos)
        {
            if (!prefs.useCompactScrollbar)
            {
                scrollbar.style.opacity = 1;
                scrollbar.style.width = 13;
                return;
            }

            var layout = root.x.layout;
            
            var max = layout.xMax;
            var min = max - 150;

            var lerp = (mousePos.x - min) / (max - min);
            
            scrollbar.style.opacity = Mathf.Lerp(0, 1, lerp);
            scrollbar.style.width = 5;
        }
        
        void DisplayOptionalParts()
        {
            toolbar?.Fluent().Display(prefs.showTabsBar);

            if (prefs.useCompactScrollbar)
                root.styleSheets.Add(scrollViewStyles);
            else
                root.styleSheets.Remove(scrollViewStyles);
        }

        
        /// <see cref="EditorElementPatch.Init_"/>
        public void SetupEditorElement(EditorElement x)
        {
            var element = x.element;
            var header = x.header;
            var inspector = x.inspector;
            var footer = x.footer;
            var data = x.Get<SubData>().First();

            var cullingEnabled = prefs.useIMGUICulling;
            
            if (x.isGo)
            {
                // Tabs bar is injected inside GO editor
                gameObjectEditor = element;
            }
            
            element.AddClass("editor-element");
            header.AddClass("header");
            inspector.AddClass("inspector");
            footer.AddClass("footer");

            header.onAddChild = e => e.AddToClassList("user-element");
            inspector.onAddChild = e => e.AddToClassList("user-element");

            SetupSubData();
            SetupEditorElement();
            SetupHeader();
            SetupInspectorElement();
            // Footer is manipulated by EditorElementPatch.Init_

            if (!editors.ContainsKey(element))
                editors.Add(element, x);
            
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
                #if UNITY_2020_1_OR_NEWER
                header.x.cullingEnabled = cullingEnabled;
                #endif
                
                if (x.isComponent)
                {
                    x.header.Direction(FlexDirection.RowReverse);
                    
                    var width = 64;
                    if (!prefs.showHelp)
                        width -= 20;
                    if (!prefs.showPreset)
                        width -= 20;
                    
                    // Temp solution for component header buttons spacing
                    if (!x.header.Has("#rightSpace", out Space space))
                    {
                        space = new Space { name = "rightSpace" };
                        x.header.x.Add(space);
                    }

                    space.style.width = width;
                }
            }
            void SetupInspectorElement()
            {
                var container = inspector.Get<IMGUIContainer>().First() ?? 
                                inspector.Get(".unity-inspector-element__custom-inspector-container").First();
                
                if (container != null)
                    SetupInspectorContainer(container);
            }

            void SetupInspectorContainer(VisualElement container)
            {
                if (container is IMGUIContainer imgui)
                {
                    #if UNITY_2020_1_OR_NEWER
                    imgui.cullingEnabled = cullingEnabled;
                    #endif
                }

                if (x.isComponent)
                {
                    // Dragging is calculated based on container layout (it ignores InspectorElement padding)
                    container.style.paddingBottom = ComponentPaddingBottom;
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