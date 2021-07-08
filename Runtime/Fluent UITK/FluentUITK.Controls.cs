using System;
using UnityEngine;
using UnityEngine.UIElements;
using FlexDir = UnityEngine.UIElements.FlexDirection;

namespace AV.UITK
{
    public enum HelpMessageType
    {
        None,
        Info,
        Warning,
        Error,
        Important,
    }
    
    public static partial class FluentUITK
    {
        public interface IHasContent // unused
        {
            Content content { get; }
        }
        
        public class Text : TextElement
        {
            public static implicit operator string(Text text) => text.text;
            
            public new string text
            {
                get => base.text;
                set
                {
                    base.text = value;
                    
                    var hasText = !string.IsNullOrEmpty(value);
                    style.display = hasText ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }

            public Text()
            {
                AddCoreStyles(this);
                AddToClassList(TextClass);
            }
            public Text(string text) : this()
            {
                this.text = text;
            }
        }
        
        /// <summary> An image with a default size of 16px. </summary>
        public class Icon : VisualElement
        {
            public static implicit operator Texture(Icon icon) => icon.image;
            
            public Texture image
            {
                get => style.backgroundImage.value.texture;
                set
                {
                    var texture = value as Texture2D;
                    style.backgroundImage = texture;
                    style.display = texture ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }
            public float size
            {
                set
                {
                    var hasValue = !Nan(value) && value > 0;
                    style.width = hasValue ? value : new StyleLength(StyleKeyword.None);
                    style.height = hasValue ? value : new StyleLength(StyleKeyword.None);
                    maxSize = value;
                }
            }
            public float maxSize
            {
                set
                {
                    var hasValue = !Nan(value) && value > 0;
                    style.maxWidth = hasValue ? value : new StyleLength(StyleKeyword.None);
                    style.maxHeight = hasValue ? value : new StyleLength(StyleKeyword.None);
                }
            }

            public Icon(Texture image = default)
            {
                AddCoreStyles(this);
                AddToClassList(IconClass);
                
                this.image = image;
            }
        }
        
        /// <summary> Analogue to <see cref="GUIContent"/> in IMGUI </summary>
        public class Content : VisualElement
        {
            public Text text { get; set; }
            public Icon icon { get; set; }
            
            // TODO: tooltip using our TooltipElement
            public Content(string text = default, Texture icon = default)
            {
                AddCoreStyles(this);
                AddToClassList(ContentClass);
                
                pickingMode = PickingMode.Ignore;
                
                this.icon = new Icon(icon);
                Add(this.icon);

                this.text = new Text(text);
                Add(this.text);
            }

            public void Set(Content other)
            {
                this.text = other?.text;
                this.icon = other?.icon;
            }
            public void Set(string text, Texture icon)
            {
                this.text.text = text;
                this.icon.image = icon;
            }
        }
        
        public class Space : VisualElement
        {
            public Space(float width = float.NaN, float height = float.NaN)
            {
                AddCoreStyles(this);
                pickingMode = PickingMode.Ignore;
                
                if (!float.IsNaN(width)) style.width = width;
                if (!float.IsNaN(height)) style.height = height;
            }
        }

        public class FlexibleSpace : VisualElement
        {
            public FlexibleSpace(float flexGrow = 1)
            {
                AddCoreStyles(this);
                pickingMode = PickingMode.Ignore;
                style.flexGrow = flexGrow;
            }
        }
        
        public class Group : VisualElement
        {
            public Group() {}
            public Group(FlexDirection direction)
            {
                AddCoreStyles(this);
                AddToClassList(GroupClass);
                style.flexDirection = direction;
            }
        }
        public class Row : Group
        {
            public Row(bool reverse = false) : base(reverse ? FlexDir.RowReverse : FlexDir.Row)
            {
                AddToClassList(RowClass);
            }
        }
        public class Column : Group
        {
            public Column(bool reverse = false) : base(reverse ? FlexDir.ColumnReverse : FlexDir.Column)
            {
                AddToClassList(ColumnClass);
            }
        }

        public class Separator : VisualElement
        {
            public Separator()
            {
                AddToClassList(SeparatorClass);
            }
        }
        public class Header : VisualElement, IHasContent
        {
            public string text { get => content.text; set => content.text.text = value; }
            public Texture icon { get => content.icon; set => content.icon.image = value; }
            public Content content { get; }

            public VisualElement container => contentContainer;
            public override VisualElement contentContainer { get; }

            public Header(string text = default, Texture icon = default)
            {
                content = new Content(text, icon);
                hierarchy.Add(content);
                
                contentContainer = new Group();
                contentContainer.AddToClassList(ContainerClass);
                
                hierarchy.Add(contentContainer);
            }
        }

        public class Button : UnityEngine.UIElements.Button, IHasContent
        {
            public new string text { get => content.text; set => content.text.text = value; }
            public Texture icon { get => content.icon; set => content.icon.image = value; }
            public Content content { get; }
            
            public Button(string text = default, Texture icon = default, Styles style = Styles.Button)
            {
                FluentUITK.DefineStyle(this, style);
                
                AddCoreStyles(this);
                AddToClassList(ButtonClass);
                RemoveFromClassList("unity-text-element");
                
                content = new Content(text, icon);
                Add(content);
            }
        }

        public class Tab : Toggle, IHasContent
        {
            public new string text { get => content.text; set => content.text.text = value; }
            public Texture icon { get => content.icon; set => content.icon.image = value; }
            public Content content { get; }

            public Tab(string text = default, Texture icon = default, Styles style = Styles.None) : base(null)
            {
                FluentUITK.DefineStyle(this, style);
                
                AddCoreStyles(this);
                AddToClassList(TabClass);
                
                this.Query(className: "unity-base-field__input").First()?.RemoveFromHierarchy();
                //RemoveFromClassList("unity-base-field");
                RemoveFromClassList("unity-base-field--no-label");
                RemoveFromClassList("unity-toggle--no-text");

                content = new Content(text, icon);
                Add(content);
            }

            #if !UNITY_2020_2_OR_NEWER
            protected override void ExecuteDefaultAction(EventBase evt)
            {
                base.ExecuteDefaultAction(evt);
                if (evt is MouseUpEvent)
                {
                    // Unbinded value doesn't change in 2019.4 and 2020.1, so do it manually
                    value = !value;
                }
            }
            #endif
        }


        public class TabsBar : VisualElement
        {
            public class PanelTab : Tab
            {
                public new FluentElement<VisualElement> panel;
                internal TabsBar bar;

                public PanelTab(VisualElement panel)
                {
                    this.panel = panel;
                }
                
                public void Show()
                {
                    bar.HideAllTabs();
                    
                    panel.Display(true);
                    SetValueWithoutNotify(true);
                }
            }
            
            // TODO: Allow Switch Off

            readonly Styles tabsStyle;

            public TabsBar(Styles tabsStyle)
            {
                this.tabsStyle = tabsStyle;
                
                AddCoreStyles(this);
                AddToClassList(TabsBarClass);
            }

            
            public FluentElement<PanelTab> AddNewTab(VisualElement panel, string text, Texture2D icon = null)
            {
                if (panel == null)
                    throw new NullReferenceException("Panel assigned to tab is null.");
                
                var tab = new PanelTab(panel) { bar = this }.Fluent();
                
                tab.Style(tabsStyle);
                tab.x.content.Set(text, icon);

                tab.OnChange<bool>(evt => tab.x.Show());
                
                Add(tab);
                return tab;
            }

            void HideAllTabs()
            {
                this.Query<PanelTab>().ForEach(x =>
                {
                    x.panel.Display(false);
                    x.SetValueWithoutNotify(false);
                });
            }
        }
        
        public class HelpBox : VisualElement, IHasContent
        {
            public string text { get => content.text; set => content.text.text = value; }
            public Texture icon { get => content.icon; set => content.icon.image = value; }
            public Content content { get; }

            public HelpBox(string text, HelpMessageType type, Texture customIcon)
            {
                AddCoreStyles(this);
                AddToClassList(HelpBoxClass);
                
                Texture icon = null;
                switch (type)
                {
                    case HelpMessageType.None: icon = GetEditorIcon("console.warnicon"); break;
                    case HelpMessageType.Info: icon = GetEditorIcon("console.infoicon"); break;
                    case HelpMessageType.Warning: icon = GetEditorIcon("console.warnicon");break;
                    case HelpMessageType.Error: icon = GetEditorIcon("console.erroricon"); break;
                    case HelpMessageType.Important: icon = GetEditorIcon("console.erroricon.inactive.sml@2x"); break;
                }
                if (customIcon)
                    icon = customIcon;
                
                content = new Content(text, icon);
                content.icon.size = 32;
                Add(content);
            }
        }
    }
}