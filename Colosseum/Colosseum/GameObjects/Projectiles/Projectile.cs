using Colosseum.GameObjects.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Projectiles
{
    abstract class Projectile : MoveableGameObject
    {
        public override bool IgnoresPlatforms { get { return true; } }
        public override bool IgnoresBounds { get { return true; } }
        public override bool IgnoresGravity { get { return true; } }

        public delegate void ProjectileStageExitHandler(object sender, EventArgs e);
        public event ProjectileStageExitHandler OnStageExit;

        public int ProjectileId { get; set; }

        public Projectile(Stage stage, Vector2 topLeftPosition, Vector2 velocity, string assetName, Texture2D texture)
            : base(stage, topLeftPosition, CreateSingleAssetDictionary(assetName, texture))
        {
            Velocity = velocity;
        }

        public Projectile(Stage stage, Vector2 topLeftPosition, Vector2 velocity, Dictionary<string, Texture2D> assetNameToTexture)
            : base(stage, topLeftPosition, assetNameToTexture)
        {
            Velocity = velocity;
        }

        public abstract Collideable ComputeCollideable();

        private static Dictionary<string, Texture2D> CreateSingleAssetDictionary(string assetName, Texture2D texture)
        {
            return new Dictionary<string, Texture2D>()
            {
                { assetName, texture }
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (TopLeftPosition.X + Width < 0 ||
                TopLeftPosition.X > Stage.Size.X ||
                TopLeftPosition.Y + Height < 0 ||
                TopLeftPosition.Y > Stage.Size.Y)
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
            return ComputeCollideable().HasCollision(fighter.ComputeCollideable());
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable());
        }
    }
}
