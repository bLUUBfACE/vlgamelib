using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SystemRectangle = System.Drawing.Rectangle;

namespace GameLibrary.Computing2D
{
    public static class RectangleExtensions
    {
        //Rectangle Conversions
        #region XnaRectangle-To-... conversions

        public static RectangleF ToRectangleF(this XnaRectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }
        public static SystemRectangle ToSystemRectangle(this XnaRectangle r)
        {
            return new SystemRectangle(r.X, r.Y, r.Width, r.Height);
        }
        public static FloatRectangle ToFloatRectangle(this XnaRectangle r)
        {
            return new FloatRectangle(r.X, r.Y, r.Width, r.Height);
        }

        #endregion

        #region FloatRectangle-To-... conversions

        public static XnaRectangle ToXnaRectangle(this FloatRectangle r)
        {
            return new XnaRectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }
        public static RectangleF ToRectangleF(this FloatRectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }

        public static SystemRectangle ToSystemRectangle(this FloatRectangle r)
        {
            return new SystemRectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }

        #endregion

        #region RectangleF-To-... conversions

        public static XnaRectangle ToXnaRectangle(this RectangleF r)
        {
            return new XnaRectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }
        public static SystemRectangle ToSystemRectangle(this RectangleF r)
        {
            return new SystemRectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }
        public static FloatRectangle ToFloatRectangle(this RectangleF r)
        {
            return new FloatRectangle(r.X, r.Y, r.Width, r.Height);
        }

        public static Polygon ToPolygon(this FloatRectangle r)
        {
            return new Polygon(new Vector2[] { new Vector2(r.X, r.Y), new Vector2(r.X + r.Width, r.Y), new Vector2(r.X + r.Width, r.Y + r.Height), new Vector2(r.X, r.Y + r.Height) }, true);
        }

        #endregion

        #region SystemRectangle-To-... conversions

        public static XnaRectangle ToXnaRectangle(this SystemRectangle r)
        {
            return new XnaRectangle(r.X, r.Y, r.Width, r.Height);
        }
        public static RectangleF ToRectangleF(this SystemRectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }
        public static FloatRectangle ToFloatRectangle(this SystemRectangle r)
        {
            return new FloatRectangle(r.X, r.Y, r.Width, r.Height);
        }

        #endregion


        #region Vector2 conversions

        public static XnaRectangle ToRectangle(this Vector2 v, int width, int height)
        {
            return new XnaRectangle((int)v.X, (int)v.Y, width, height);
        }
        public static RectangleF ToRectangleF(this Vector2 v, float width, float height)
        {
            return new RectangleF(v.X, v.Y, width, height);
        }
        public static SystemRectangle ToSystemRectangle(this Vector2 v, int width, int height)
        {
            return new SystemRectangle((int)v.X, (int)v.Y, width, height);
        }
        public static FloatRectangle ToFloatRectangle(this Vector2 v, float width, float height)
        {
            return new FloatRectangle(v, width, height);
        }

        #endregion

        #region IntersectionArea functions

        public static XnaRectangle IntersectionArea(this XnaRectangle r1, XnaRectangle r2)
        {
            return SystemRectangle.Intersect(r1.ToSystemRectangle(), r2.ToSystemRectangle()).ToXnaRectangle();
        }
        public static SystemRectangle IntersectionArea(this SystemRectangle r1, SystemRectangle r2)
        {
            return SystemRectangle.Intersect(r1, r2);
        }
        public static FloatRectangle IntersectionArea(this FloatRectangle r1, FloatRectangle r2)
        {
            float x = Math.Max(r1.X, r2.X), y = Math.Max(r1.Y, r2.Y);
            FloatRectangle returnRectangle = new FloatRectangle(x, y, Math.Min(r1.Right, r2.Right) - x, Math.Min(r1.Bottom, r2.Bottom) - y);
            return returnRectangle.Width > 0 && returnRectangle.Height > 0 ? returnRectangle : FloatRectangle.Empty;
        }
        public static RectangleF IntersectionArea(this RectangleF r1, RectangleF r2)
        {
            float x = Math.Max(r1.X, r2.X), y = Math.Max(r1.Y, r2.Y);
            RectangleF returnRectangle = new RectangleF(x, y, Math.Min(r1.Right, r2.Right) - x, Math.Min(r1.Bottom, r2.Bottom) - y);
            return returnRectangle.Width > 0 && returnRectangle.Height > 0 ? returnRectangle : RectangleF.Empty;
        }

        #endregion

        #region CenterRectangle functions

        public static XnaRectangle CenterRectangle(this XnaRectangle r, int border)
        {
            return r.ToRectangleF().CenterRectangle(border).ToXnaRectangle();
        }
        public static FloatRectangle CenterRectangle(this FloatRectangle r, float border)
        {
            return r.ToRectangleF().CenterRectangle(border).ToFloatRectangle();
        }
        public static SystemRectangle CenterRectangle(this SystemRectangle r, int border)
        {
            return r.ToRectangleF().CenterRectangle(border).ToSystemRectangle();
        }
        public static RectangleF CenterRectangle(this RectangleF r, float border)
        {
            if (border >= r.Width / 2 && border >= r.Height / 2)
                throw new System.ArgumentException();
            return new RectangleF(r.X + border, r.Y + border, r.Width - 2 * border, r.Height - 2 * border);
        }

        #endregion

        #region Absolute Functions

        public static FloatRectangle Abs(this FloatRectangle r)
        {
            return new FloatRectangle(r.Width < 0 ? r.X + r.Width : r.X, r.Height < 0 ? r.Y + r.Height : r.Y, Math.Abs(r.Width), Math.Abs(r.Height));
        }
        public static XnaRectangle Abs(this XnaRectangle r)
        {
            return new XnaRectangle(r.Width < 0 ? r.X + r.Width : r.X, r.Height < 0 ? r.Y + r.Height : r.Y, Math.Abs(r.Width), Math.Abs(r.Height));
        }
        public static RectangleF Abs(this RectangleF r)
        {
            return new RectangleF(r.Width < 0 ? r.X + r.Width : r.X, r.Height < 0 ? r.Y + r.Height : r.Y, Math.Abs(r.Width), Math.Abs(r.Height));
        }
        public static SystemRectangle Abs(this SystemRectangle r)
        {
            return new SystemRectangle(r.Width < 0 ? r.X + r.Width : r.X, r.Height < 0 ? r.Y + r.Height : r.Y, Math.Abs(r.Width), Math.Abs(r.Height));
        }

        #endregion

        public static PointF Center(this RectangleF r)
        {
            return new PointF(r.X + r.Width / 2, r.Y + r.Height / 2);
        }
        public static Vector2 Center(this FloatRectangle r)
        {
            return new Vector2(r.X + r.Width / 2, r.Y + r.Height / 2);
        }
        public static Microsoft.Xna.Framework.Point Center(this XnaRectangle r)
        {
            return new Microsoft.Xna.Framework.Point(r.X + r.Width / 2, r.Y + r.Height / 2);
        }
    }
}
