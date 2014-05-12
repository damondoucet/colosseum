using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum.GameObjects.Collisions
{
    abstract class Collideable
    {
        public abstract List<Vector2> TestPoints { get; }
        public abstract bool Contains(Vector2 vector);

        public bool HasCollision(Collideable rhs)
        {
            return TestPoints.Any(rhs.Contains) || rhs.TestPoints.Any(Contains);
        }
    }
}
