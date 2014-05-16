using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Colosseum.GameObjects.Attacks
{
    abstract class Attack : MoveableGameObject
    {
        public abstract bool AbsorbsAttacks { get; }
        public abstract bool IsDeadly { get; }

        public Attack(Stage stage, Vector2 position, string assetName)
            : base(stage, position, assetName)
        {
        }


        public abstract Collideable ComputeCollideable();

        public virtual void ExitStage()
        {
            Stage.RemoveAttack(this);
        }

        public virtual bool HasCollisionWithFighter(Fighter fighter)
        {
            return ComputeCollideable().HasCollision(fighter.ComputeCollideable());
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable());
        }
    }
}
