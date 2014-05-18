using Colosseum.GameObjects.Attacks.Melee;
using Colosseum.GameObjects.Collisions;
using Colosseum.Graphics;
using Colosseum.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Fighters
{
    class Knight : Fighter
    {
        private static List<string> KnightAssetNames = new List<string>()
        {
            Constants.Assets.FighterHead,
            Constants.Assets.FighterBody,
            Constants.Assets.FighterWeapon
        };

        private string _weaponAsset;

        protected override string HeadAsset { get { return Constants.Assets.FighterHead; } }
        protected override string BodyAsset { get { return Constants.Assets.FighterBody; } }
        protected override string WeaponAsset { get { return _weaponAsset; } }

        protected override float DashVelocity { get { return Constants.Fighters.Knight.DashVelocity; } }
        protected override float TotalDashTime { get { return Constants.Fighters.Knight.DashTime; } }

        private readonly Dictionary<FighterInputDispatcher.Action, Action> _buttonToAbility;
        protected override Dictionary<FighterInputDispatcher.Action, Action> ButtonToAbility { get { return _buttonToAbility; } }

        public bool IsSwingingSword { get; set; }

        private KnightShield _shield;  // null if not in the knight's posession

        public Knight(Stage stage, Vector2 position, float weaponAngle)
            : base(stage, position, weaponAngle)
        {
            _weaponAsset = Constants.Assets.FighterWeapon;

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
            base.HandleAction(action, pressed, leftThumbstick, rightThumbstick);

            // TODO: :/
            // TODO: ignores cooldown
            if (action == FighterInputDispatcher.Action.LeftShoulder && !pressed)
                StopBlocking();
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
            _weaponAsset = Constants.Assets.KnightThrust;
            Stage.AddAttack(new KnightThrust(this));
        }

        public void OnThrustFinished()
        {
            _weaponAsset = Constants.Assets.FighterWeapon;
            IsSwingingSword = false;
            Cooldown = Constants.Fighters.Knight.Abilities.Thrust.Cooldown;
        }

        private void Block()
        {
            if (IsSwingingSword || _shield.CurrentState == KnightShield.State.Shielding)
                return;

            _shield.CurrentState = KnightShield.State.Shielding;
            _shield.ShieldAngle = WeaponAngle;
            Stage.AddAttack(_shield);
        }

        private void StopBlocking()
        {
            _shield.CurrentState = KnightShield.State.Stored;
            _shield.ExitStage();
        }

        protected override List<Asset> ComputeAssets()
        {
            var assets = base.ComputeAssets();

            if (_shield.CurrentState == KnightShield.State.Shielding)
                assets.RemoveAt(assets.Count - 1);  // remove the weapon... this is an awful hack

            return assets;
        }

        private void ThrowShield()
        {
            if (_shield.CurrentState != KnightShield.State.Stored)
                return;

            Console.WriteLine("throw shield");
        }

        public Collideable ComputeWeaponCollideable()
        {
            // weapon is rotated a specific angle; to go from the original top left corner to the center
            // you need to go rotate a little more
            // although honestly I'm not really sure why it's pi/2 in this case
            var angle = WeaponAngle + Math.PI / 2;
            var angleVector = Util.VectorFromAngle(angle);

            var weaponSize = TextureDictionary.FindTextureSize(WeaponAsset);
            var center = TopLeftPosition + ComputeWeaponOffset() + angleVector * weaponSize / 2.0f;

            return new Rect(center, weaponSize.X, weaponSize.Y, angle);
        }
    }
}
