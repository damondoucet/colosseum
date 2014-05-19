using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Collisions
{
    class NonCollideable : Collideable
    {
        public override List<Vector2> TestPoints { get { return new List<Vector2>(); } }

        public override bool Contains(Vector2 vector)
        {
            return false;
        }
    }
}
