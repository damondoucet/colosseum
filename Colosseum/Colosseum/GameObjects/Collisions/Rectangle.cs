using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum.GameObjects.Collisions
{
    class Rect : Collideable
    {
        private readonly Vector2 _center;
        private readonly float _halfWidth;
        private readonly float _halfHeight;
        private readonly double _angle;

        private readonly List<Vector2> _testPoints;
        public override List<Vector2> TestPoints { get { return _testPoints; } }

        public override bool Contains(Vector2 vector)
        {
            // invert the transformation we did earlier on the corners
            // then check if this point is contained in what the normal bounding box would be
            var inverted = Util.RotateAboutOrigin(vector - _center, -(_angle - MathHelper.PiOver2));

            return Math.Abs(inverted.X) <= _halfWidth &&
                Math.Abs(inverted.Y) <= _halfHeight;
        }

        /// <summary>
        /// Represents a square with a distance from the center to the corner.
        /// The angle is the angle of one of the corners
        /// </summary>
        public Rect(Vector2 center, float width, float height, double angle)
        {
            _center = center;
            _angle = angle;

            _halfWidth = width / 2.0f;
            _halfHeight = height / 2.0f;
            
            _testPoints = CreateAxisAlignedPoints()
                .Select(pt => center + Util.RotateAboutOrigin(pt, angle - MathHelper.PiOver2))
                .ToList();
        }

        private List<Vector2> CreateAxisAlignedPoints()
        {
            var points = new List<Vector2>();

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (!(i == 0 && j == 0))
                        points.Add(new Vector2(_halfWidth* i, _halfHeight* j));

            return points;
        }
    }
}
