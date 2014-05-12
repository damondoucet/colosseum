using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum.GameObjects.Collisions
{
    class CompoundCollideable : Collideable
    {
        private readonly List<Collideable> _collideables;

        public override List<Vector2> TestPoints { get { return _collideables.SelectMany(c => c.TestPoints).ToList(); } }

        public override bool Contains(Vector2 vector)
        {
            return _collideables.Any(c => c.Contains(vector));
        }

        public CompoundCollideable(IEnumerable<Collideable> collideables)
        {
            _collideables = collideables.ToList();
        }
    }
}
