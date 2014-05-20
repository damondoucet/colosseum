using Colosseum.GameObjects.Attacks.Melee;
using Colosseum.GameObjects.Attacks.Projectiles;
using Colosseum.GameObjects.Collisions;
using Colosseum.Graphics;
using Colosseum.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Fighters
{
    class Knight : Fighter, Thruster
    {
        private static List<string> KnightAssetNames = new List<string>()
        {
            Constants.GameAssets.Knight.Head,
            Constants.GameAssets.Knight.Body,
            Constants.GameAssets.Knight.Weapon
        };

        private string _weaponAsset;

        public override string HeadAsset { get { return Constants.GameAssets.Knight.Head; } }
        public override string BodyAsset { get { return Constants.GameAssets.Knight.Body; } }
        public override string WeaponAsset { get { return _weaponAsset; } }

        protected override float DashVelocity { get { return Constants.Fighters.Knight.DashVelocity; } }
        protected override float TotalDashTime { get { return Constants.Fighters.Knight.DashTime; } }
        protected override float DashCooldown { get { return Constants.Fighters.Knight.DashCooldown; } }

        private readonly Dictionary<FighterInputDispatcher.Action, Action> _buttonToAbility;
        protected override Dictionary<FighterInputDispatcher.Action, Action> ButtonToAbility { get { return _buttonToAbility; } }

        public bool IsSwingingSword { get; set; }

        private KnightShield _shield;  // null if not in the knight's posession

        public Knight(Stage stage, Vector2 position, float weaponAngle)
            : base(stage, position, weaponAngle)
        {
            _weaponAsset = Constants.GameAssets.Knight.Weapon;

            // WARNING: if you change the block button, you need to change it in HandleAction below
            // because damon sucks and doesn't have time to do this all the right way
            _buttonToAbility = new Dictionary<FighterInputDispatcher.Action, Action>()
            {
                { FighterInputDispatcher.Action.RightShoulder, SwingSword },
                { FighterInputDispatcher.Action.RightTrigger, Thrust },
                { FighterInputDispatcher.Action.LeftShoulder, Block },
                { FighterInputDispatcher.Action.LeftTrigger, ThrowShield }
            };

            _shield = new KnightShield(this);
        }

        public override void OnRightThumbstick(Vector2 value)
        {
            if (_shield.CurrentState == KnightShield.State.Shielding)
                return;

            var angleIsLeft = Util.IsAngleLeft(Math.Atan2(value.Y, value.X));
            
            if (!IsSwingingSword)
                base.OnRightThumbstick(value);
            else if (value.X != 0 || value.Y != 0) // &&  // otherwise we only let them change sides
            {
                if ((angleIsLeft && !IsFacingLeft()) ||
                    (!angleIsLeft && IsFacingLeft()))
                    FlipWeaponAngle();
            }
        }

        private void FlipWeaponAngle()
        {
            WeaponAngle = (float)(Math.PI - WeaponAngle);
        }

        protected override bool CanPerformAction()
        {
            return base.CanPerformAction() && !IsSwingingSword;
        }

        public override void HandleAction(FighterInputDispatcher.Action action, bool pressed, Vector2 leftThumbstick, Vector2 rightThumbstick)
        {
            // TODO: :/
            if (action == FighterInputDispatcher.Action.LeftShoulder && !pressed)
                StopBlocking();
            else if (_shield.CurrentState == KnightShield.State.Shielding)
                return;

            base.HandleAction(action, pressed, leftThumbstick, rightThumbstick);

        }

        private void SwingSword()
        {
            if (IsSwingingSword || _shield.CurrentState == KnightShield.State.Shielding)
                return;

            IsSwingingSword = true;

            var textureSize = TextureDictionary.FindTextureSize(WeaponAsset);
            Stage.AddAttack(new KnightSwordSwing(this, (int)textureSize.X, (int)textureSize.Y));
        }

        public void OnSwingSwordFinished()
        {
            IsSwingingSword = false;
            WeaponAngle = IsFacingLeft() ? (float)Math.PI : 0;
            Cooldown = Constants.Fighters.Knight.Abilities.SwordSwing.Cooldown;
        }

        private void Thrust()
        {
            if (IsSwingingSword || _shield.CurrentState == KnightShield.State.Shielding)
                return;

            IsSwingingSword = true;
            _weaponAsset = Constants.GameAssets.Knight.Thrust;
            Stage.AddAttack(ThrustFactory.CreateKnightThrust(this));
        }

        public void OnThrustFinished()
        {
            _weaponAsset = Constants.GameAssets.Knight.Weapon;
            IsSwingingSword = false;
            Cooldown = Constants.Fighters.Knight.Abilities.Thrust.Cooldown;
        }

        private void Block()
        {
            if (IsSwingingSword || _shield.CurrentState != KnightShield.State.Stored)
                return;

            _shield.Velocity = Vector2.Zero;
            _shield.CurrentState = KnightShield.State.Shielding;
            Stage.AddAttack(_shield);
        }

        private void StopBlocking()
        {
            if (_shield.CurrentState != KnightShield.State.Shielding)
                return;

            _shield.CurrentState = KnightShield.State.Stored;
            _shield.ExitStage();
        }

        private void ThrowShield()
        {
            if (_shield.CurrentState == KnightShield.State.Stored)
                _shield.Throw(WeaponAngle);
        }

        protected override List<Asset> ComputeAssets()
        {
            var assets = base.ComputeAssets();

            if (_shield.CurrentState == KnightShield.State.Shielding)
                assets.RemoveAt(assets.Count - 1);  // remove the weapon... this is an awful hack

            return assets;
        }
    }
}
