using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Melee
{
    abstract class MeleeAttack : Attack
    {
        protected override bool PersistsAfterFirstHit { get { return true; } }

        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public MeleeAttack(Fighter source)
            : base(source, Vector2.Zero)  // these handle their own movement
        { 
        }
    }

    // this sucks. a lot. :/ (diamond problem)
    abstract class TimedMeleeAttack : TimedAttack
    {
        protected override bool PersistsAfterFirstHit { get { return true; } }

        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public TimedMeleeAttack(Fighter source)
            : base(source, Vector2.Zero)  // these handle their own movement
        { 
        }
    }
}
