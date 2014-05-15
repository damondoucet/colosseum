using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum.GameObjects.Collisions
{
    // TODO: rectangle?
    class Square : Collideable
    {
        private readonly Vector2 _center;
        private readonly float _halfSideLength;
        private readonly double _angle;

        private readonly List<Vector2> _testPoints;
        public override List<Vector2> TestPoints { get { return _testPoints; } }

        public override bool Contains(Vector2 vector)
        {
            // invert the transformation we did earlier on the corners
            // then check if this point is contained in what the normal bounding box would be
            var inverted = Util.RotateAboutOrigin(vector - _center, -(_angle - MathHelper.PiOver2));

            return Math.Abs(inverted.X) <= _halfSideLength &&
                Math.Abs(inverted.Y) <= _halfSideLength;
        }

        /// <summary>
        /// Represents a square with a distance from the center to the corner.
        /// The angle is the angle of one of the corners
        /// </summary>
        public Square(Vector2 center, float cornerDistance, double angle)
        {
            _center = center;
            _halfSideLength = (float)(cornerDistance / Math.Sqrt(2));
            _angle = angle;

            _testPoints = CreateAxisAlignedPoints(_halfSideLength)
                .Select(pt => center + Util.RotateAboutOrigin(pt, angle - MathHelper.PiOver2))
                .ToList();
        }

        private List<Vector2> CreateAxisAlignedPoints(float halfSideLength)
        {
            var points = new List<Vector2>();

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (!(i == 0 && j == 0))
                        points.Add(new Vector2(halfSideLength * i, halfSideLength * j));

            return points;
        }
    }
}
