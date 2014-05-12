using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum.GameObjects.Collisions
{
    class Triangle : Collideable
    {
        private readonly List<Vector2> _corners;

        private readonly List<Vector2> _testPoints;
        public override List<Vector2> TestPoints { get { return _testPoints; } }

        //http://www.blackpawn.com/texts/pointinpoly/
        public override bool Contains(Vector2 vector)
        {
            var v0 = _corners[1] - _corners[0];
            var v1 = _corners[2] - _corners[0];
            var v2 = vector - _corners[0];

            var dot00 = v0.Dot(v0);
            var dot01 = v0.Dot(v1);
            var dot02 = v0.Dot(v2);
            var dot11 = v1.Dot(v1);
            var dot12 = v1.Dot(v2);

            var invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return u >= 0 && v >= 0 && u + v <= 1;
        }

        /// <summary>
        /// Represents a triangle inscribed in a circle with a given radius. 
        /// One of the triangle's points will be at the given angle
        /// </summary>
        public Triangle(Vector2 center, float radius, double angle)
        {
            // 6 test points: the corners of the triangles, and the midpoints of the sides
            _corners = CreateAxisAlignedCorners(radius)
                .Select(c => center + Util.RotateAboutOrigin(c, angle))
                .ToList();

            _testPoints = _corners.Concat(ComputeMidpoints(_corners))
                .ToList();
        }

        private List<Vector2> CreateAxisAlignedCorners(float radius)
        {
            return new List<Vector2>()
            {
                radius * Util.VectorFromAngle(0),
                radius * Util.VectorFromAngle(2 * Math.PI / 3.0f),
                radius * Util.VectorFromAngle(4 * Math.PI / 3.0f)
            };
        }

        private List<Vector2> ComputeMidpoints(List<Vector2> corners)
        {
            return new List<Vector2>
            {
                (corners[0] + corners[1]) / 2.0f,
                (corners[1] + corners[2]) / 2.0f,
                (corners[2] + corners[0]) / 2.0f
            };
        }
    }
}
