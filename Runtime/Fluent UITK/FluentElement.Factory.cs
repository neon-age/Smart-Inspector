
using UnityEngine;
using UnityEngine.UIElements;
using Space = AV.UITK.FluentUITK.Space;
using Button = AV.UITK.FluentUITK.Button;

using UIE = UnityEngine.UIElements;
using static AV.UITK.FluentUITK;

namespace AV.UITK
{
    public partial class FluentElement<T>
    {
        internal FluentElement<T> AddCoreStyles()
        {
            FluentUITK.AddCoreStyles(x); return x;
        }

        public FluentElement NewElement()
        {
            return new VisualElement();
        }
        public FluentElement<TElement> New<TElement>() where TElement : VisualElement, new()
        {
            return new TElement();
        }
        
        public FluentElement<Text> NewText(string text)
        {
            return new Text(text);
        }
        
        public FluentElement<Icon> NewIcon(Texture background)
        {
            return new Icon(background);
        }


        public FluentElement<FluentUITK.HelpBox> NewHelpBox(string text, HelpMessageType type, Texture customIcon = null)
        {
            return new FluentUITK.HelpBox(text, type, customIcon);
        }
        
        public FluentElement<Separator> NewSeparator()
        {
            return new Separator();
        }
        public FluentElement<Header> NewHeader(string text, Texture icon = null)
        {
            return new Header(text, icon);
        }
        public FluentElement<Foldout> NewFoldout(string text)
        {
            return new Foldout { text = text };
        }
        
        public FluentElement<Row> NewRow(bool reverse = false)
        {
            return new Row(reverse);
        }
        public FluentElement<Column> NewColumn(bool reverse = false)
        {
            return new Column(reverse);
        }
        public FluentElement<Group> NewGroup()
        {
            return new Group();
        }
        public FluentElement<Group> NewIndentedGroup(int left = 15, int right = 1, int top = 0, int bottom = 0,
            bool useMargin = false, FlexDirection direction = UnityEngine.UIElements.FlexDirection.Column)
        {
            var group = new Group(direction).Fluent();
            
            if (useMargin)
                group.Margin(top, left, right, bottom);
            else
                group.Padding(top, left, right, bottom);
            
            return group;
        }
        
        
        public FluentElement<Space> NewSpace(float width = float.NaN, float height = float.NaN)
        {
            return new Space(width, height);
        }
        public FluentElement<FlexibleSpace> NewFlexibleSpace(float flexGrow = 1)
        {
            return new FlexibleSpace(flexGrow);
        }
        
        
        public FluentElement<FluentUITK.TabsBar> NewTabsBar(Styles tabsStyle)
        {
            return new FluentUITK.TabsBar(tabsStyle);
        }


        public FluentElement<FluentUITK.Tab> NewTab(string text = null, Texture icon = null, Styles style = Styles.None)
        {
            return new FluentUITK.Tab(text, icon, style);
        }

        
        public FluentElement<Button> NewButton(string text = null, Texture icon = null, Styles style = Styles.None)
        {
            return new Button(text, icon, style);
        }
        
        
        public FluentElement<Text> NewBullet()
        {
            return NewText("•").AddClass("bullet");
        }
        public FluentElement<Text> NewHyperlink(string text, string url)
        {
            var link = NewText(text)
                .OnClick(() => Application.OpenURL(url), MouseButton.LeftMouse)
                .AddClass("hyperlink");
            
            link.x.tooltip = url;
            
            #if UNITY_2020_1_OR_NEWER
            link.TextOverflow(UIE.TextOverflow.Ellipsis);
            #endif

            return link;
        }
    }
}