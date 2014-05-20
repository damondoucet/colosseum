using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class WizardCloudProjectile : Projectile
    {
        protected override double PhaseInTime { get { return Constants.Fighters.Wizard.Abilities.CloudProjectile.PhaseInTime; } }
        protected override double TimeToLive { get { return Constants.Fighters.Wizard.Abilities.CloudProjectile.TimeToLive; } }

        public override int Width { get { return Constants.Fighters.Wizard.Abilities.CloudProjectile.Width; } }
        public override int Height { get { return Constants.Fighters.Wizard.Abilities.CloudProjectile.Height; } }

        public override bool IgnoresGravity { get { return false; } }
        public override bool IgnoresPlatforms { get { return false; } }
        public override bool IgnoresBounds { get { return false; } }

        public WizardCloudProjectile(Fighter source, Vector2 topLeftPosition)
            : base(source, topLeftPosition, Vector2.Zero)
        {
        }

        protected override List<Asset> ComputeAssets()
        {
            return new Asset(Stage, Constants.GameAssets.Wizard.CloudProjectile, TopLeftPosition).SingleToList();
        }

        public override Collideable ComputeCollideable()
        {
            return new Rect(ComputeCenter(), Width, Height, 0);
        }

        public override void OnPlatformCollision(Vector2 contactVector)
        {
            ExitStage();
        }
    }
}
