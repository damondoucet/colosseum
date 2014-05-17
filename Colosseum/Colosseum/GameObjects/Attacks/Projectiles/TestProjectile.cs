using Colosseum.GameObjects.Collisions;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class TestProjectile : Projectile
    {
        public TestProjectile(Stage stage, Vector2 topLeftPosition, Vector2 velocity)
            : base(stage, topLeftPosition, velocity)
        {
        }

        protected override double TimeToLive { get { return Constants.Projectiles.Test.PhaseInTime; } }
        protected override double PhaseInTime { get { return Constants.Projectiles.Test.PhaseInTime; } }

        public override int Width { get { return Constants.Projectiles.Test.Width; } }
        public override int Height { get { return Constants.Projectiles.Test.Height; } }

        private float ComputeAngle()
        {
            return (float)Math.Atan2(FireVelocity.Y, FireVelocity.X);
        }

        protected override List<Asset> ComputeAssets()
        {
            return new List<Asset>()
            {
                new Asset(Stage, Constants.Assets.TestProjectile, TopLeftPosition, ComputeAngle())
            };
        }

        public override Collideable ComputeCollideable()
        {
            return new Triangle(
                TopLeftPosition + new Vector2(Width, Height) / 2.0f,
                Width / 2.0f,
                ComputeAngle());
        }
    }
}
