using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.GameObjects.Attacks.Melee
{
    abstract class MeleeAttack : Attack
    {
        public override bool AbsorbsAttacks { get { return false; } }
        public override bool IsDeadly { get { return true; } }

        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public MeleeAttack(Stage stage, Vector2 position)
            : base(stage, position)
        { 
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            // don't actually draw; the fighter should do the drawing for us
            // still need to draw the hitbox if necessary though
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable());
        }
    }

    // this sucks. a lot. :/ (diamond problem)
    abstract class TimedMeleeAttack : TimedAttack
    {
        public override bool AbsorbsAttacks { get { return false; } }
        public override bool IsDeadly { get { return true; } }

        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public TimedMeleeAttack(Stage stage, Vector2 position)
            : base(stage, position)
        {
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            // don't actually draw; the fighter should do the drawing for us
            // still need to draw the hitbox if necessary though
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable());
        }
    }
}
