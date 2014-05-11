using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Projectiles
{
    class ProjectileFactory
    {
        private int _counter;
        private static Dictionary<string, Texture2D> AssetNameToTexture;

        private readonly Stage _stage;

        public ProjectileFactory(Stage stage)
        {
            _stage = stage;
            _counter = 0;
        }

        private void LoadAsset(ContentManager content, string assetName)
        { 
            AssetNameToTexture.Add(
                Constants.Assets.TestProjectile, 
                content.Load<Texture2D>(Constants.Assets.TestProjectile));
        }

        public void LoadContent(ContentManager content)
        {
            AssetNameToTexture = new Dictionary<string, Texture2D>();

            LoadAsset(content, Constants.Assets.TestProjectile);
        }

        public void CreateTestProjectile(Vector2 topLeftPosition, Vector2 velocity)
        { 
            var projectile = new TestProjectile(_stage, topLeftPosition, velocity, AssetNameToTexture);
            projectile.ProjectileId = _counter++;
            projectile.OnStageExit += OnProjectileStageExit;

            _stage.AddProjectile(projectile);
        }

        private void OnProjectileStageExit(object sender, EventArgs e)
        {
            _stage.RemoveProjectile(((Projectile)sender).ProjectileId);
        }
    }
}
