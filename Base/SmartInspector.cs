using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class SmartInspector
    {
        public class EditorElement
        {
            public VisualElement element;
            public VisualElement inspector;
            public Editor editor;
            public int index;
            
            public Object target => editor.target;
        }
        
        internal static SmartInspector LastInjected;
        
        internal EditorWindow propertyEditor;
        internal ActiveEditorTracker tracker;
        internal VisualElement root;
        internal VisualElement mainContainer;
        internal VisualElement editorsList;
        internal VisualElement gameObjectEditor;

        internal List<EditorElement> editors;

        SmartInspector(EditorWindow propertyEditor)
        {
            this.propertyEditor = propertyEditor;
            this.tracker = PropertyEditorRef.GetTracker(propertyEditor);

            this.root = propertyEditor.rootVisualElement;
            this.mainContainer = root.Query(className: "unity-inspector-main-container").First();
            this.editorsList = root.Query(className: "unity-inspector-editors-list").First();
        }

        public static bool TryInject(EditorWindow propertyEditor, out SmartInspector inspector)
        {
            inspector = new SmartInspector(propertyEditor);
            return inspector.TryInject();
        }
        
        public bool TryInject()
        {
            LastInjected = this;
            
            if (editorsList == null)
                return false;
            
            root.styleSheets.Add(UIResources.Asset.scrollViewStyle);
            editorsList.styleSheets.Add(UIResources.Asset.componentsHeaderStyle);
            
            var scrollView = root.Query(className: "unity-inspector-root-scrollview").First();
            var contentViewport = scrollView.Get("unity-content-viewport");
            
            contentViewport.RegisterCallback<GeometryChangedEvent>(_ => contentViewport.style.marginRight = 0);
            
            RetrieveEditorElements();

            var hasMainToolbar = mainContainer.Query<InspectorMainToolbar>().HasAny();
            if (!hasMainToolbar)
            {
                var inspectorToolbar = new InspectorMainToolbar(propertyEditor);
                inspectorToolbar.RegisterCallback<DetachFromPanelEvent>(_ => InspectorInjection.TryReinjectWindow(propertyEditor));
                
                //mainContainer.Insert(0, inspectorToolbar);
            }
            
            if (gameObjectEditor == null)
                return false;
            
            //var inspectorElement = gameObjectEditor.Query<InspectorElement>().First();
            //if (inspectorElement == null)
            //    return false; // Wrong editor
            
            if (gameObjectEditor.childCount != 3)
                return false; // Uninitialized
            
            //var inspectorObject = PropertyEditorRef.GetInspectedObject(propertyEditor);
            //Debug.Log(inspectorObject);
            
            var componentsToolbar = new InspectorComponentsToolbar();
            componentsToolbar.RegisterCallback<DetachFromPanelEvent>(_ => InspectorInjection.TryReinjectWindow(propertyEditor));
            
            // Insert after InspectorElement and before Footer
            gameObjectEditor.Insert(2, componentsToolbar);
            
            //var gameObjectFooter = gameObjectEditor.Query<InspectorElement>().First();
            
            return true;
        }

        /// <see cref="PropertyEditorPatch.RebuildContentsContainers_"/>
        public void OnRebuildContent(EditorWindow window, InspectorInjection.RebuildStage stage)
        {
            if (window != propertyEditor)
                return;

            if (stage == InspectorInjection.RebuildStage.EndBeforeRepaint)
            {
                RebuildToolbar();
                FixComponentLayout();
            }

            if (stage == InspectorInjection.RebuildStage.PostfixAfterRepaint)
            {
                FixComponentLayout();
            }
        }

        void FixComponentLayout()
        {
            // Fixes an issue when collapsed components kept expanded layout (no idea why!)
            editorsList.Query(className: "component").ForEach(x =>
            {
                x.style.fontSize = 12 + (Random.value / 1000);
            });
        }
        
        public void RebuildToolbar()
        {
            RetrieveEditorElements();
            var toolbar = root.Query<InspectorComponentsToolbar>().First();
            toolbar.Rebuild();
        }

        void RetrieveEditorElements()
        {
            this.editors = new List<EditorElement>();
            
            var allEditors = editorsList.Children().ToList();
            
            foreach (var editorElement in allEditors)
            {
                if (editorElement.GetType() != EditorElementRef.type)
                    continue;

                var inspector = editorElement.Get(".unity-inspector-element");
                if (inspector == null)
                    continue;
                
                var editor = EditorElementRef.GetEditor(editorElement);
                var editorIndex = EditorElementRef.GetEditorIndex(editorElement);
                
                var target = editor.target;
                var isGo = target is GameObject;
                
                inspector.RegisterCallback<GeometryChangedEvent>(evt =>
                {
                    var isExpanded = InternalEditorUtility.GetIsInspectorExpanded(target);
                    editorElement.EnableInClassList("is-expanded", isExpanded);
                });

                if (isGo)
                    gameObjectEditor = editorElement;
                    
                editorElement.EnableInClassList("game-object", isGo);
                editorElement.EnableInClassList("transform", target is Transform);
                editorElement.EnableInClassList("component", target is Component);
                editorElement.EnableInClassList("material", target is Material);

                if (!isGo)
                    editors.Add(new EditorElement
                    {
                        element = editorElement, 
                        inspector = inspector,
                        editor = editor,
                        index = editorIndex
                    });
            }
        }
        
        /*
        private static void ManipulateEditorElement(VisualElement editorElement, Editor editor)
        {
            var header = editorElement?.Query<IMGUIContainer>().Where(x => x != null && x.name.EndsWith("Header")).First();
            var footer = editorElement?.Query<IMGUIContainer>().Where(x => x != null && x.name.EndsWith("Footer")).First();
            
            if (header == null || footer == null)
                return;
            
            // Can't use EditorElement:hover for some reason, ignore footer hovering instead
            footer.pickingMode = PickingMode.Ignore;
            
            //header.style.display = DisplayStyle.None; // Can't use display, as other inspector methods rely on that
            header.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                header.onGUIHandler = null;
                header.style.height = 0;
            });

            var componentHeader = editorElement.Query<ComponentHeaderElement>().First();
            
            if (componentHeader == null)
            {
                componentHeader = new ComponentHeaderElement(editorElement, editor);
                header.parent.Insert(0, componentHeader);
            }

            //header.RemoveFromHierarchy();
        }*/
    }
}