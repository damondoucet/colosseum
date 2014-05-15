using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Colosseum.GameObjects.Projectiles
{
    abstract class Projectile : MoveableGameObject
    {
        public abstract float PhaseInTime { get; }
        public abstract float TimeToLive { get; }

        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public delegate void ProjectileStageExitHandler(object sender, EventArgs e);
        public event ProjectileStageExitHandler OnStageExit;

        public int ProjectileId { get; set; }

        private float _timeAlive;
        protected Vector2 FireVelocity;

        public Projectile(Stage stage, Vector2 topLeftPosition, Vector2 velocity, string assetName)
            : base(stage, topLeftPosition, assetName)
        {
            FireVelocity = velocity;
            _timeAlive = 0;

            Velocity = Vector2.Zero;
        }
        
        public abstract Collideable ComputeCollideable();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _timeAlive += dt;

            if (_timeAlive - dt < PhaseInTime && _timeAlive >= PhaseInTime)
                Velocity = FireVelocity;

            if (TopLeftPosition.X + Width < 0 ||
                TopLeftPosition.X > Stage.Size.X ||
                TopLeftPosition.Y + Height < 0 ||
                TopLeftPosition.Y > Stage.Size.Y || 
                _timeAlive > TimeToLive + PhaseInTime)
                ExitStage();
        }

        public void ExitStage()
        {
            if (OnStageExit == null)
                Console.WriteLine("WARNING: Projectile.OnStageExit is null. This will cause a memory leak!");
            else
                OnStageExit(this, null);
        }

        public bool HasCollisionWithFighter(Fighter fighter)
        {
            return _timeAlive > PhaseInTime && 
                ComputeCollideable().HasCollision(fighter.ComputeCollideable());
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable());
        }
    }
}
