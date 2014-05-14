using Colosseum.GameObjects.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Projectiles
{
    class TestProjectile : Projectile
    {
        public TestProjectile(Stage stage, Vector2 topLeftPosition, Vector2 velocity, Dictionary<string, Texture2D> assetNameToTexture)
            : base(stage, topLeftPosition, velocity, Constants.Assets.TestProjectile, assetNameToTexture[Constants.Assets.TestProjectile])
        {
        }

        public override float TimeToLive { get { return Constants.Projectiles.Test.PhaseInTime; } }
        public override float PhaseInTime { get { return Constants.Projectiles.Test.PhaseInTime; } }

        public override int Width { get { return Constants.Projectiles.Test.Width; } }
        public override int Height { get { return Constants.Projectiles.Test.Height; } }

        private float ComputeAngle()
        {
            return (float)Math.Atan2(FireVelocity.Y, FireVelocity.X);
        }

        public override float GetAssetRotation(string assetName)
        {
            return ComputeAngle();
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
