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
        
        private readonly Stage _stage;

        public ProjectileFactory(Stage stage)
        {
            _stage = stage;
            _counter = 0;
        }

        public void CreateTestProjectile(Vector2 topLeftPosition, Vector2 velocity)
        { 
            var projectile = new TestProjectile(_stage, topLeftPosition, velocity);
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
