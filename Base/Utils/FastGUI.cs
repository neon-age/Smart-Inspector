using UnityEngine;

namespace AV.Inspector
{
    internal static class FastGUI
    {
        private static Material guiMaterial;
        
        public static void DrawRect(Rect rect, Color color)
        {
            if (!guiMaterial)
                guiMaterial = new Material(Shader.Find("Hidden/Internal-GUITextureClip"));
            
            var sourceRect = new Rect(0, 0, 1f, 1f);
            Graphics.DrawTexture(rect, Texture2D.whiteTexture, sourceRect, 0, 0, 0, 0, color, guiMaterial, -1);
        }
        
        public static void DrawTexture(Rect rect, Texture image, Color color, Material mat)
        {
            var sourceRect = new Rect(0, 0, 1f, 1f);
            
            //TextureUtil.GetUsageMode(image) == TextureUsageMode.AlwaysPadded
            // In case of always padded textures, only show the non-padded area
            //sourceRect.width *= position.width / TextureUtil.GetGPUWidth(image);
            //sourceRect.height *= position.height / TextureUtil.GetGPUHeight(image);
            
            Graphics.DrawTexture(rect, image, sourceRect, 0, 0, 0, 0, color, mat, -1);
        }
    }
}