using Colosseum.GameObjects.Attacks.Melee;
using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class WizardForcePulse : TimedMeleeAttack
    {
        private readonly Wizard _wizard;

        private readonly double _angle;

        private double _width;
        public override int Width { get { return (int)_width; } }
        public override int Height { get { return Constants.Fighters.Wizard.Abilities.ForcePulse.Height; } }

        protected override double PhaseInTime { get { return Constants.Fighters.Wizard.Abilities.ForcePulse.PhaseInTime; } }
        protected override double TimeToLive { get { return Constants.Fighters.Wizard.Abilities.ForcePulse.PhaseInTime; } }

        public WizardForcePulse(Wizard wizard, double angle)
            : base(wizard)
        {
            _wizard = wizard;
            _angle = angle;
        }

        public override void Update(GameTime gameTime)
        {
            TopLeftPosition = _wizard.TopLeftPosition + _wizard.ComputeWeaponOffset() - new Vector2(0, Height / 2);
            _width += gameTime.ElapsedGameTime.TotalSeconds * Constants.Fighters.Wizard.Abilities.ForcePulse.Scale;

            base.Update(gameTime);
        }

        public override void ExitStage()
        {
            _wizard.OnForcePulseFinished();
            base.ExitStage();
        }

        public override Collideable ComputeCollideable()
        {
            return new Rect(ComputeCenter(), Width, Height, _angle);
        }

        public override bool HasCollisionWithFighter(Fighter fighter)
        {
            return fighter != _wizard && base.HasCollisionWithFighter(fighter);
        }

        public override void OnFighterCollision(Fighter fighter)
        {
            AddKnockbackToFighter(fighter);
        }

        private void AddKnockbackToFighter(Fighter fighter)
        {
            // TODO: this can be refactored; same code exists in Explosion
            var vector = (fighter.ComputeCenter() - ComputeCenter()).Norm();
            var force = vector * Constants.Fighters.Wizard.Abilities.ForcePulse.KnockbackForce;
            var kb = new KnockbackForce(_wizard, fighter, Constants.Fighters.Wizard.Abilities.ForcePulse.KnockbackTime, force);
            Stage.AddAttack(kb);
        }

        protected override List<Asset> ComputeAssets()
        {
            return new Asset(
                Stage, 
                Constants.GameAssets.Wizard.ForcePulse, 
                TopLeftPosition, 
                new Vector2(Width, Height), 
                (float)_angle, 
                Color.White, 
                SpriteEffects.None
            ).SingleToList();
        }
    }
}
