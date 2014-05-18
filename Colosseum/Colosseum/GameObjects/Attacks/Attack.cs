using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.GameObjects.Attacks
{
    abstract class Attack : MoveableGameObject
    {
        public Attack(Stage stage, Vector2 position)
            : base(stage, position)
        {
        }

        public abstract Collideable ComputeCollideable();

        protected virtual bool ShouldExit()
        {
            return false;
        }

        public virtual void ExitStage()
        {
            Stage.RemoveAttack(this);
        }

        public virtual bool HasCollisionWithFighter(Fighter fighter)
        {
            return ComputeCollideable().HasCollision(fighter.ComputeCollideable());
        }

        public virtual bool HasCollisionWithAttack(Attack attack)
        {
            return false;
        }

        public virtual void OnFighterCollision(Fighter fighter)
        {
            fighter.OnHit();
        }

        public virtual void OnAttackCollision(Attack attack)
        { 
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable(), ComputeCenter());
        }

        public override void Update(GameTime gameTime)
        {
            if (ShouldExit())
                ExitStage();

            base.Update(gameTime);
        }
    }
}
