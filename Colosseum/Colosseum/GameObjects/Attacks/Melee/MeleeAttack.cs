using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;

namespace Colosseum.GameObjects.Attacks.Melee
{
    abstract class MeleeAttack : Attack
    {
        protected override bool PersistsAfterFirstHit { get { return true; } }

        public override bool CollisionIgnoresSource { get { return true; } }

        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public MeleeAttack(Fighter source)
            : base(source, Vector2.Zero)  // these handle their own movement
        { 
        }

        public override bool HasCollisionWithFighter(Fighter fighter)
        {
            return (!CollisionIgnoresSource || Source != fighter) &&
                base.HasCollisionWithFighter(fighter);
        }
    }

    // this sucks. a lot. :/ (diamond problem)
    abstract class TimedMeleeAttack : TimedAttack
    {
        protected override bool PersistsAfterFirstHit { get { return true; } }

        public override bool CollisionIgnoresSource { get { return true; } }

        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public TimedMeleeAttack(Fighter source)
            : base(source, Vector2.Zero)  // these handle their own movement
        { 
        }

        public override bool HasCollisionWithFighter(Fighter fighter)
        {
            return (!CollisionIgnoresSource || Source != fighter) &&
                base.HasCollisionWithFighter(fighter);
        }
    }
}
