using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class WizardTriangleProjectile : Projectile
    {
        public WizardTriangleProjectile(Fighter source, Vector2 topLeftPosition, Vector2 velocity)
            : base(source, topLeftPosition, velocity)
        {
        }

        protected override double TimeToLive { get { return Constants.Fighters.Wizard.Abilities.Triangle.PhaseInTime; } }
        protected override double PhaseInTime { get { return Constants.Fighters.Wizard.Abilities.Triangle.PhaseInTime; } }

        public override int Width { get { return Constants.Fighters.Wizard.Abilities.Triangle.Width; } }
        public override int Height { get { return Constants.Fighters.Wizard.Abilities.Triangle.Height; } }

        private float ComputeAngle()
        {
            var vector = TimeAlive < PhaseInTime ? FireVelocity : Velocity;  // velocity can change, e.g. when projectile is deflected
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        protected override List<Asset> ComputeAssets()
        {
            return new Asset(Stage, Constants.GameAssets.TestProjectile, TopLeftPosition, ComputeAngle())
                .SingleToList();
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
