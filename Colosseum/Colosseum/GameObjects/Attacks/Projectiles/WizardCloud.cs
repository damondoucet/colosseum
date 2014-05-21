using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class WizardCloud : Projectile
    {
        private readonly Wizard _wizard;

        private double _timeUntilNextProjectile;

        protected override double PhaseInTime { get { return Constants.Fighters.Wizard.Abilities.Cloud.PhaseInTime; } }
        protected override double TimeToLive { get { return Constants.Fighters.Wizard.Abilities.Cloud.TimeToLive; } }

        public override int Width { get { return Constants.Fighters.Wizard.Abilities.Cloud.Width; } }
        public override int Height { get { return Constants.Fighters.Wizard.Abilities.Cloud.Height; } }

        public WizardCloud(Wizard wizard, Vector2 position, Vector2 velocity)
            : base(wizard, position, velocity)
        {
            _wizard = wizard;
            _timeUntilNextProjectile = Constants.Fighters.Wizard.Abilities.Cloud.TimeBetweenProjectiles;
        }

        public override void ExitStage()
        {
            _wizard.OnCloudExit();
            base.ExitStage();
        }

        public override Collideable ComputeCollideable()
        {
            return new NonCollideable();
        }

        protected override List<Asset> ComputeAssets()
        {
            return new Asset(Stage, Constants.GameAssets.Wizard.Cloud, TopLeftPosition).SingleToList();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (TimeAlive < PhaseInTime)
                return;

            _timeUntilNextProjectile -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeUntilNextProjectile < 0)
            {
                SpawnProjectile();
                _timeUntilNextProjectile += Constants.Fighters.Wizard.Abilities.Cloud.TimeBetweenProjectiles;
            }
        }

        private void SpawnProjectile()
        { 
            var position = TopLeftPosition + new Vector2(Width / 2, Height);
            Stage.AddAttack(new WizardCloudProjectile(_wizard, position));
        }
    }
}
