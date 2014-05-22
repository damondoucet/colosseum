using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using System;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class KnockbackForce : Projectile
    {
        private readonly MoveableGameObject _target;
        private readonly double _timeToLive;
        private readonly Vector2 _force;

        protected override double PhaseInTime { get { return 0; } }
        protected override double TimeToLive { get { return _timeToLive; } }

        public override int Width { get { return 0; } }
        public override int Height { get { return 0; } }

        public KnockbackForce(Fighter source, MoveableGameObject target, double timeToLive, Vector2 force)
            : base(source, Vector2.Zero, Vector2.Zero)
        {
            _target = target;
            _timeToLive = timeToLive;
            _force = force;
        }

        public override void Update(GameTime gameTime)
        {
            // if we're on the platform, only go up, not down
            var force = _target.IsSittingOnPlatform() ? new Vector2(_force.X, Math.Min(0, _force.Y)) : _force;

            _target.TopLeftPosition += force * (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override Collideable ComputeCollideable()
        {
            return new NonCollideable();
        }
    }
}
