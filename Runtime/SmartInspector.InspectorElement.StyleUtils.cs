using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public partial class InspectorElement
        {
            const float n = float.NaN;
            
            public void SetFlexDirection(FlexDirection direction) => x.style.flexDirection = direction;
            
            public void SetPosition(Position type, float top = n, float left = n, float right = n, float bottom = n) => x.style.SetPosition(type, top, left, right, bottom);
            public void SetPosition(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetPosition(top, left, right, bottom);
            
            public void AlignSelf(Align align) => x.style.alignSelf = align;
            public void AlignItems(Align align) => x.style.alignItems = align;
            public void AlignContent(Align align) => x.style.alignContent = align;
            
            public void SetSize(float width = n, float height = n) => x.style.SetSize(width, height);
            
            public void SetMargin(float margin) => x.style.SetMargin(margin);
            public void SetMargin(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetMargin(top, left, right, bottom);

            public void SetPadding(float padding) => x.style.SetPadding(padding);
            public void SetPadding(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetPadding(top, left, right, bottom);

            public void SetBorderColor(Color color) => x.style.SetBorderColor(color);
            
            public void SetBorderWidth(float width) => x.style.SetBorderWidth(width);
            public void SetBorderWidth(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetBorderWidth(top, left, right, bottom);
            
            public void SetBorderRadius(float radius) => x.style.SetBorderRadius(radius);
            public void SetBorderRadius(float top = n, float left = n, float right = n, float bottom = n) => x.style.SetBorderRadius(top, left, right, bottom);
        }
    }
}