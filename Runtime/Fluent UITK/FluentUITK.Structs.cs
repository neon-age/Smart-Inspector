using UnityEngine;
using UnityEngine.UIElements;

namespace AV.UITK
{
    public static partial class FluentUITK
    {
        const float n = float.NaN;
        static bool Nan(float value) => float.IsNaN(value);
        
        public struct Positioning
        {
            public Position position;
            public Sides sides;

            public float top => sides.top;
            public float left => sides.left;
            public float right => sides.right;
            public float bottom => sides.bottom;

            public Positioning(Position position, Sides sides)
            {
                this.position = position;
                this.sides = sides;
            }
        }
        
        public struct Sides
        {
            public float top;
            public float left;
            public float right;
            public float bottom;
        
            public Sides(Sides x) : this(x.top, x.left, x.right, x.bottom) {}
            public Sides(float top, float left, float right, float bottom)
            {
                this.top = top;
                this.left = left;
                this.right = right;
                this.bottom = bottom;
            }
        
            public static implicit operator Vector4(Sides x) => new Vector4(x.top, x.left, x.right, x.bottom);
            public static implicit operator Sides(Vector4 x) => new Sides(x.x, x.y, x.z, x.w);
        }
        public struct SidesTopLeft
        {
            public float top;
            public float left;
        
            public SidesTopLeft(SidesTopLeft x) : this(x.top, x.left) {}
            public SidesTopLeft(float top, float left)
            {
                this.top = top;
                this.left = left;
            }
        
            public static implicit operator Vector2(SidesTopLeft x) => new Vector2(x.top, x.left);
            public static implicit operator SidesTopLeft(Vector2 x) => new SidesTopLeft(x.x, x.y);
        }
        public struct SidesColor
        {
            public Color top;
            public Color left;
            public Color right;
            public Color bottom;

            public SidesColor(SidesColor x) : this(x.top, x.left, x.right, x.bottom) {}
            public SidesColor(Color top, Color left, Color right, Color bottom)
            {
                this.top = top;
                this.left = left;
                this.right = right;
                this.bottom = bottom;
            }
        }
        
        
        public struct Corners
        {
            public float topLeft;
            public float topRight;
            public float bottomLeft;
            public float bottomRight;
        
            public Corners(Corners x) : this(x.topLeft, x.topRight, x.bottomLeft, x.bottomRight) {}
            public Corners(float topLeft, float topRight, float bottomLeft, float bottomRight)
            {
                this.topLeft = topLeft;
                this.topRight = topRight;
                this.bottomLeft = bottomLeft;
                this.bottomRight = bottomRight;
            }

            public Corners ToSides(float top = n, float left = n, float right = n, float bottom = n)
            {
                var c = new Corners(this);
                if (!Nan(top)) c.topLeft = c.topRight = top;
                if (!Nan(left)) c.topLeft = c.bottomLeft = left;
                if (!Nan(right)) c.topRight = c.bottomRight = right;
                if (!Nan(bottom)) c.bottomLeft = c.bottomRight = bottom;
                return c;
            }
        
            public static implicit operator Vector4(Corners x) => new Vector4(x.topLeft, x.topRight, x.bottomLeft, x.bottomRight);
            public static implicit operator Corners(Vector4 x) => new Corners(x.x, x.y, x.z, x.w);
        }
        public struct CornersColor
        {
            public Color topLeft;
            public Color topRight;
            public Color bottomLeft;
            public Color bottomRight;

            public CornersColor(CornersColor x) : this(x.topLeft, x.topRight, x.bottomLeft, x.bottomRight) {}
            public CornersColor(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
            {
                this.topLeft = topLeft;
                this.topRight = topRight;
                this.bottomLeft = bottomLeft;
                this.bottomRight = bottomRight;
            }
        }
    }
}