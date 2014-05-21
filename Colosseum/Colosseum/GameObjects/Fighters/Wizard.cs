using Colosseum.GameObjects.Attacks.Projectiles;
using Colosseum.Graphics;
using Colosseum.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Fighters
{
    class Wizard : Fighter
    {
        public override string StandardHeadAsset { get { return Constants.GameAssets.Wizard.Head; } }
        public override string StandardBodyAsset { get { return Constants.GameAssets.Wizard.Body; } }
        public override string StandardWeaponAsset { get { return Constants.GameAssets.Wizard.Weapon; } }
        public override string StunnedHeadAsset { get { return Constants.GameAssets.Wizard.StunnedHead; } }

        protected override float DashVelocity { get { return Constants.Fighters.Wizard.DashVelocity; } }
        protected override float TotalDashTime { get { return Constants.Fighters.Wizard.DashTime; } }
        protected override float DashCooldown { get { return Constants.Fighters.Wizard.DashCooldown; } }

        private readonly Dictionary<FighterInputDispatcher.Action, Action> _buttonToAbility;
        protected override Dictionary<FighterInputDispatcher.Action, Action> ButtonToAbility { get { return _buttonToAbility; } }

        private WizardBomb _wizardBomb;  // null if none currently exists
        private bool _canUseCloud;

        private bool _isUsingForcePulse;

        public Wizard(Stage stage, Vector2 topLeftPosition, float weaponAngle)
            : base(stage, topLeftPosition, weaponAngle)
        {
            _canUseCloud = true;
            _isUsingForcePulse = false;

            _buttonToAbility = new Dictionary<FighterInputDispatcher.Action, Action>()
            {
                { FighterInputDispatcher.Action.LeftShoulder, SpawnOrDetonateBomb },
                { FighterInputDispatcher.Action.RightShoulder, FireTriangle },
                { FighterInputDispatcher.Action.LeftTrigger, SpawnCloud },
                { FighterInputDispatcher.Action.RightTrigger, ForcePulse }
            };
        }

        private Vector2 ComputeProjectileStartPosition()
        {
            var bodyCenter = TopLeftPosition + new Vector2(Width, Height) / 2.0f;

            var weaponSize = TextureDictionary.FindTextureSize(WeaponAsset);
            var dist = Constants.Fighters.Wizard.Abilities.Triangle.FireDistance;

            var radius = Width / 2.0f + Constants.Fighters.WeaponDistance + weaponSize.X + dist;

            return bodyCenter + radius * Util.VectorFromAngle(WeaponAngle) - new Vector2(0, Constants.Fighters.Wizard.Abilities.Triangle.Height / 2.0f);
        }

        private void SpawnOrDetonateBomb()
        {
            if (_wizardBomb == null)
            {
                SpawnBomb();
                Cooldown += Constants.Fighters.Wizard.Abilities.Bomb.Cooldown;
            }
            else if (_wizardBomb.CanBeDetonated())
            {
                _wizardBomb.ExitStage();  // automatically spawns explosion
                OnBombDetonated();
                Cooldown += Constants.Fighters.Wizard.Abilities.Bomb.Cooldown;
            }
        }

        private void SpawnBomb()
        { 
            var velocity = Constants.Fighters.Wizard.Abilities.Bomb.VelocityMagnitude * Util.VectorFromAngle(WeaponAngle);

            _wizardBomb = new WizardBomb(this, ComputeProjectileStartPosition(), velocity);
            Stage.AddAttack(_wizardBomb);
        }

        public void OnBombDetonated()
        {
            _wizardBomb = null;
        }

        private void FireTriangle()
        {
            var velocity = Constants.Fighters.Wizard.Abilities.Triangle.VelocityMagnitude * Util.VectorFromAngle(WeaponAngle);
            var position = ComputeProjectileStartPosition();
            Stage.AddAttack(new WizardTriangleProjectile(this, position, velocity));
            Cooldown += Constants.Fighters.Wizard.Abilities.Triangle.Cooldown;
        }

        private void SpawnCloud()
        {
            if (!_canUseCloud)
                return;

            _canUseCloud = false;

            var velocity = Constants.Fighters.Wizard.Abilities.Cloud.VelocityMagnitude * Util.VectorFromAngle(WeaponAngle);
            var yOffset = new Vector2(0, Constants.Fighters.Wizard.Abilities.Cloud.YOffset);

            Stage.AddAttack(new WizardCloud(this, ComputeProjectileStartPosition() + yOffset, velocity));
        }

        public void OnCloudExit()
        {
            _canUseCloud = true;
        }

        private void ForcePulse()
        {
            _isUsingForcePulse = true;

            Stage.AddAttack(new WizardForcePulse(this, WeaponAngle));
        }

        public void OnForcePulseFinished()
        {
            _isUsingForcePulse = false;
            Cooldown += Constants.Fighters.Wizard.Abilities.ForcePulse.Cooldown;
        }

        protected override bool CanMove()
        {
            return base.CanPerformAction() && !_isUsingForcePulse;
        }
    }
}
