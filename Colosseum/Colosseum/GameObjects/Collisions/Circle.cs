using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum.GameObjects.Collisions
{
    class Circle : Collideable
    {
        private const int NumTestPoints = 6;

        private readonly Vector2 _center;
        private readonly float _radiusSquared;

        private readonly List<Vector2> _testPoints;
        public override List<Vector2> TestPoints { get { return _testPoints; } }

        public override bool Contains(Vector2 vector)
        {
            return (vector - _center).LengthSquared() <= _radiusSquared;
        }

        static bool b = false;
        public Circle(Vector2 center, float radius)
        {
            _center = center;
            _radiusSquared = radius * radius;

            _testPoints = Enumerable.Range(0, NumTestPoints)
                .Select(i => center + radius * Util.VectorFromAngle(2 * i * Math.PI / NumTestPoints))
                .ToList();
        }
    }
}
