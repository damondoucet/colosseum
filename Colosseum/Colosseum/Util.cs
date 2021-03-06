using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum
{
    static class Util
    {
        public static Vector2 VectorFromAngle(double angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static Vector2 RotateAboutOrigin(Vector2 vector, double angle)
        {
            var x = (float)(vector.X * Math.Cos(angle) - vector.Y * Math.Sin(angle));
            var y = (float)(vector.X * Math.Sin(angle) + vector.Y * Math.Cos(angle));

            return new Vector2(x, y);
        }

        public static float Dot(this Vector2 lhs, Vector2 rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y;
        }

        public static bool IsAngleLeft(double angle)
        {
            return Math.Cos(angle) < 0;  // heh...
        }

        public static List<T> SingleToList<T>(this T t)
        {
            return new List<T>() { t };
        }

        // really annoying that their Normalize isn't functional
        public static Vector2 Norm(this Vector2 v)
        {
            v.Normalize();
            return v;
        }
    }
}
