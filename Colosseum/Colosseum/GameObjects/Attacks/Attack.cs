using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks
{
    abstract class Attack : MoveableGameObject
    {
        public readonly Fighter Source;

        public abstract bool CollisionIgnoresSource { get; }

        protected abstract bool PersistsAfterFirstHit { get; }

        protected List<Fighter> FightersHit;

        public Attack(Fighter source, Vector2 position)
            : base(source.Stage, position)
        {
            Source = source;

            FightersHit = new List<Fighter>();
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
            return (!CollisionIgnoresSource || Source != fighter) &&
                !FightersHit.Contains(fighter) &&
                ComputeCollideable().HasCollision(fighter.ComputeCollideable());
        }

        public virtual bool HasCollisionWithAttack(Attack attack)
        {
            return false;
        }

        public virtual void OnFighterCollision(Fighter fighter)
        {
            fighter.OnHit(this);
            FightersHit.Add(fighter);

            if (!PersistsAfterFirstHit)
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
