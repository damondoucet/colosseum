using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.GameObjects.Attacks.Melee
{
    abstract class MeleeAttack : Attack
    {
        protected virtual bool ShouldDraw { get { return false; } }
        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public MeleeAttack(Stage stage)
            : base(stage, Vector2.Zero)  // these handle their own movement
        { 
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            if (ShouldDraw)  // sometimes the fighter draws the weapon for us
                base.Draw(batch, gameTime);
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable());
        }
    }

    // this sucks. a lot. :/ (diamond problem)
    abstract class TimedMeleeAttack : TimedAttack
    {
        protected virtual bool ShouldDraw { get { return false; } }
        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public TimedMeleeAttack(Stage stage)
            : base(stage, Vector2.Zero)  // these handle their own movement
        { 
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            if (ShouldDraw)  // sometimes the fighter draws the weapon for us
                base.Draw(batch, gameTime);
            else  // Attack.Draw handles hitbox for us
                HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable());
        }
    }
}
