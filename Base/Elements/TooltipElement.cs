using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AV.UITK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    internal class TooltipElement : VisualElement
    {
        internal const int DefaultShowDelayMS = 100;
        const int HideDelayTickMS = 50;
        const int HideDelayTicksCount = 3;
        
        const int MoveMS = 100;
        const int FitMS = 50;
        const int FadeInOutMS = 50;
        
        public enum Align
        {
            Top,
            Bottom
        }

        static readonly Color BackgroundColor = EditorGUIUtility.isProSkin ? new Color32(42, 42, 42, 255) : new Color32(240, 240, 240, 255);
        static readonly Color BorderColor = EditorGUIUtility.isProSkin ? new Color32(54, 54, 54, 255) : new Color32(200, 200, 200, 255);
        
        readonly TextElement text = new TextElement();
        readonly VisualElement fitter = new VisualElement { name = "Fitter" };
        
        new string tooltip;
        Vector2 topLeft;
        Rect textRect;
        Rect worldClip;
        
        int hideTicks;
        bool wantsToBeHidden;

        static TooltipElement shownTooltip;
        
        public TooltipElement()
        {
            styleSheets.Add(SmartInspector.tooltipStyles);
            style.opacity = 0;

            fitter.Add(text);
            Add(fitter);
            
            this.Query<VisualElement>().ForEach(x => x.pickingMode = PickingMode.Ignore);

            text.style.backgroundColor = BackgroundColor;
            text.style.SetBorderColor(BorderColor);
            
            this.RegisterCallback<GeometryChangedEvent>(OnLayoutChange);
            text.RegisterCallback<GeometryChangedEvent>(OnTextLayout);
        }

        void OnLayoutChange(GeometryChangedEvent evt)
        {
            worldClip = this.GetWorldClip();
        }
        void OnTextLayout(GeometryChangedEvent evt)
        {
            textRect = evt.newRect;
        }

        void FitToWorldClip(int fitMS = FitMS)
        {
            var bounds = new Rect(layout.position + textRect.position, textRect.size);

            var fit = Vector2.zero;

            if (bounds.xMin < worldClip.xMin)
                fit.x = -(worldClip.xMin + bounds.xMin);
            
            if (bounds.xMax > worldClip.xMax)
                fit.x = worldClip.xMax - bounds.xMax;

            // TODO: Fit for top
            fitter.experimental.animation.TopLeft(fit, fitMS);
        }

        static async void AsyncUpdate()
        {
            while (shownTooltip != null)
            {
                await Task.Delay(10);
                shownTooltip?.FitToWorldClip();
            }
        }

        public void Hide()
        {
            DelayHide();
            
            wantsToBeHidden = true;
        }
        
        public void ShowAt(Rect rect, string tooltip, Align align = Align.Bottom, int showDelayMS = DefaultShowDelayMS)
        {
            this.topLeft = rect.center;
            this.tooltip = tooltip;

            // Not perfect, but it works with one-line tooltips
            if (align == Align.Top)
            {
                topLeft.y = rect.yMin;
                text.style.top = Length.Percent(-180);
            }
            if (align == Align.Bottom)
            {
                topLeft.y = rect.yMax;
                text.style.top = Length.Percent(-70);
            }

            if (resolvedStyle.opacity == 0)
            {
#pragma warning disable 4014
                DelayShow(showDelayMS);
#pragma warning restore 4014
            }
            else
                ShowAnimated();
            
            wantsToBeHidden = false;
        }
        
        async Task DelayShow(int delayMS)
        {
            await Task.Delay(delayMS);
            
            if (wantsToBeHidden)
                return;
            
            ShowAnimated();
        }
        async void DelayHide()
        {
            hideTicks = HideDelayTicksCount;
            
            while (hideTicks > 0)
            {
                hideTicks--;
                await Task.Delay(HideDelayTickMS);
                
                if (!wantsToBeHidden)
                    return;
            }
            
            shownTooltip = null;
            
            var opacity = resolvedStyle.opacity;
            experimental.animation.Start(opacity, 0, FadeInOutMS, (e, value) => e.style.opacity = value);
        }
        

        void ShowAnimated()
        {
            if (shownTooltip == null)
            {
                shownTooltip = this;
                AsyncUpdate();
            }

            wantsToBeHidden = false;

            var opacity = resolvedStyle.opacity;
            
            if (opacity == 0)
            {
                style.left = topLeft.x;
                style.top = topLeft.y;
                
                FitToWorldClip(fitMS: 0);
            }
            
            text.text = tooltip;
            
            experimental.animation.TopLeft(topLeft, MoveMS);
            experimental.animation.Start(opacity, 1, FadeInOutMS, (e, value) => e.style.opacity = value);
        }
    }
}
