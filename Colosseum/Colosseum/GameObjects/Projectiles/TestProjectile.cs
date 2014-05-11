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
        public override int Width { get { return Constants.Projectiles.Test.Width; } }
        public override int Height { get { return Constants.Projectiles.Test.Height; } }

        public override float GetAssetRotation(string assetName)
        {
            return (float)Math.Atan2(Velocity.Y, Velocity.X);
        }
    }
}
