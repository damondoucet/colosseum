using Colosseum.GameObjects.Attacks.Melee;
using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class WizardBomb : Projectile
    {
        private readonly Wizard _wizard;

        protected override double PhaseInTime { get { return Constants.Fighters.Wizard.Abilities.Bomb.PhaseInTime; } }
        protected override double TimeToLive { get { return Constants.Fighters.Wizard.Abilities.Bomb.TimeToLive; } }

        public override int Width { get { return Constants.Fighters.Wizard.Abilities.Bomb.Width; } }
        public override int Height { get { return Constants.Fighters.Wizard.Abilities.Bomb.Height; } }

        public WizardBomb(Wizard wizard, Vector2 topLeftPosition, Vector2 velocity)
            : base(wizard, topLeftPosition, velocity)
        {
            _wizard = wizard;
        }

        public bool CanBeDetonated()
        {
            return TimeAlive > PhaseInTime;
        }

        public override Collideable ComputeCollideable()
        {
            return new NonCollideable();
        }

        public override void ExitStage()
        {
            base.ExitStage();
            Stage.AddAttack(new Explosion(Source, ComputeCenter()));
            _wizard.OnBombDetonated();
        }

        protected override List<Asset> ComputeAssets()
        {
            return new List<Asset>()
            {
                new Asset(Stage, Constants.GameAssets.Wizard.Bomb, TopLeftPosition)
            };
        }
    }
}
