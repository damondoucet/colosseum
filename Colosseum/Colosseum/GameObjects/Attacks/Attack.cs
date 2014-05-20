using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.GameObjects.Attacks
{
    abstract class Attack : MoveableGameObject
    {
        public readonly Fighter Source;

        public Attack(Fighter source, Vector2 position)
            : base(source.Stage, position)
        {
            Source = source;
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
            fighter.OnHit(this);
            ExitStage();
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
