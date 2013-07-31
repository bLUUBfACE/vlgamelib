using System;
using Microsoft.Xna.Framework;
using System.Drawing;
using XnaPoint = Microsoft.Xna.Framework.Point;
using DrawingPoint = System.Drawing.Point;

namespace GameLibrary.Computing2D
{
    public static class Vector2Extensions
    {
        public static Vector2 ToAbs(this Vector2 v)
        {
            return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
        }

        public static double Scalar(this Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        #region Angles between vectors

        public static float AngleInDegrees(this Vector2 v1)
        {
            return v1.AngleInDegrees(new Vector2(1, 0));
        }
        public static float AngleInDegrees(this Vector2 v1, Vector2 v2)
        {
            return 360f - (float)MathHelper.ToDegrees((float)Math.Acos(v1.Scalar(v2) / (v1.Length() * v2.Length())));
        }

        public static float AngleInRadians(this Vector2 v1)
        {
            return v1.AngleInRadians(new Vector2(1, 0));
        }
        public static float AngleInRadians(this Vector2 v1, Vector2 v2)
        {
            return 360f - (float)Math.Acos(v1.Scalar(v2) / (v1.Length() * v2.Length()));
        }

        public static float AngleInDegreesAntiClockwise(this Vector2 v1, Vector2 v2)
        {
            //return 360f - Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(v1.X * v2.Y - v1.Y * v2.X, v1.X * v2.X + v1.Y * v2.Y)));
            float rotation = MathHelper.ToDegrees((float)(Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X)));
            return rotation < 0 ? 360f - Math.Abs(rotation) : rotation;
        }
        public static float AngleInRadiansAntiClockwise(this Vector2 v1, Vector2 v2)
        {
            //return MathHelper.TwoPi - Math.Abs((float)Math.Atan2(v1.X * v2.Y - v1.Y * v2.X, v1.X * v2.X + v1.Y * v2.Y));
            float rotation = (float)(Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X));
            return rotation < 0 ? MathHelper.TwoPi - Math.Abs(rotation) : rotation;
        }

        #endregion

        public static Vector2 RotateDeg(this Vector2 v, float degrees)
        {
            float radians = MathHelper.ToRadians(degrees);
            return new Vector2((float)(Math.Cos(radians) * v.X - Math.Sin(radians) * v.Y), (float)(Math.Sin(radians) * v.X + Math.Cos(radians) * v.Y));
        }
        public static Vector2 RotateRad(this Vector2 v, float radians)
        {
            return new Vector2((float)(Math.Cos(radians) * v.X - Math.Sin(radians) * v.Y), (float)(Math.Sin(radians) * v.X + Math.Cos(radians) * v.Y));
        }

        public static Vector2 Reflect(this Vector2 v, Vector2 mirror)
        {
            mirror.Normalize(); mirror *= v.Length();
            return mirror.RotateRad(v.AngleInRadiansAntiClockwise(mirror));
        }

        public static float DistanceSquared(this Vector2 v1, Vector2 v2)
        {
            return (v1 - v2).LengthSquared();
        }

        #region Point-To-Vector2 conversions

        public static Vector2 ToVector2(this PointF p)
        {
            return new Vector2(p.X, p.Y);
        }
        public static Vector2 ToVector2(this Microsoft.Xna.Framework.Point p)
        {
            return new Vector2(p.X, p.Y);
        }
        public static Vector2 ToVector2(this System.Drawing.Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        #endregion

        #region Vector2-To-Point conversions
        public static XnaPoint ToXnaPoint(this Vector2 v)
        {
            return new XnaPoint((int)v.X, (int)v.Y);
        }
        public static DrawingPoint ToDrawingPoint(this Vector2 v)
        {
            return new DrawingPoint((int)v.X, (int)v.Y);
        }
        public static PointF ToPointF(this Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }
        #endregion
    }
}
