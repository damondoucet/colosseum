using Colosseum.GameObjects.Attacks.Melee;
using Colosseum.GameObjects.Collisions;
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

        private readonly Dictionary<InputHelper.Action, Action> _buttonToAbility;
        protected override Dictionary<InputHelper.Action, Action> ButtonToAbility { get { return _buttonToAbility; } }

        public bool IsSwingingSword { get; set; }

        public Knight(Stage stage, Vector2 position, float weaponAngle)
            : base(stage, position, weaponAngle)
        {
            _weaponAsset = Constants.Assets.FighterWeapon;

            _buttonToAbility = new Dictionary<InputHelper.Action, Action>()
            {
                { InputHelper.Action.RightShoulder, SwingSword },
                { InputHelper.Action.RightTrigger, Thrust },
                { InputHelper.Action.LeftShoulder, Block },
                { InputHelper.Action.LeftTrigger, ThrowShield }
            };
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

        private void SwingSword()
        {
            if (IsSwingingSword)
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
            if (IsSwingingSword)
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
            Console.WriteLine("block");
        }

        private void ThrowShield()
        {
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
