using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    abstract class Projectile : TimedAttack
    {
        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }
        
        protected Vector2 FireVelocity;

        public Projectile(Fighter source, Vector2 topLeftPosition, Vector2 velocity)
            : base(source, topLeftPosition)
        {
            FireVelocity = velocity;

            Velocity = Vector2.Zero;
        }

        public override bool HasCollisionWithFighter(Fighter fighter)
        {
            return TimeAlive > PhaseInTime && 
                base.HasCollisionWithFighter(fighter);
        }

        protected override bool ShouldExit()
        {
            return TopLeftPosition.X + Width < 0 ||
                TopLeftPosition.X > Stage.Size.X ||
                TopLeftPosition.Y + Height < 0 ||
                TopLeftPosition.Y > Stage.Size.Y ||
                base.ShouldExit();
        }

        protected override void OnPhaseInCompleted()
        {
            Velocity = FireVelocity;
        }
    }
}
