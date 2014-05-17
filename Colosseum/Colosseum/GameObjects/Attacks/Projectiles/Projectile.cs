using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    abstract class Projectile : TimedAttack
    {
        public override bool AbsorbsAttacks { get { return false; } }
        public override bool IsDeadly { get { return true; } }
        
        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }
        
        private float _timeAlive;
        protected Vector2 FireVelocity;

        public Projectile(Stage stage, Vector2 topLeftPosition, Vector2 velocity, string assetName)
            : base(stage, topLeftPosition, assetName)
        {
            FireVelocity = velocity;
            _timeAlive = 0;

            Velocity = Vector2.Zero;
        }

        public override bool HasCollisionWithFighter(Fighter fighter)
        {
            return _timeAlive > PhaseInTime && 
                base.HasCollisionWithFighter(fighter);
        }

        protected override bool ShouldExit()
        {
            return TopLeftPosition.X + Width < 0 ||
                TopLeftPosition.X > Stage.Size.X ||
                TopLeftPosition.Y + Height < 0 ||
                TopLeftPosition.Y > Stage.Size.Y ||
                _timeAlive > TimeToLive + PhaseInTime;
        }

        protected override void OnPhaseInCompleted()
        {
            Velocity = FireVelocity;
        }
    }
}
