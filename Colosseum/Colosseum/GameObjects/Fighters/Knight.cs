using Colosseum.GameObjects.Attacks.Melee;
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

        public string GetWeaponAsset() { return _weaponAsset; }
        public void SetWeaponAsset(string weaponAsset) { _weaponAsset = weaponAsset; }

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
                { InputHelper.Action.RightTrigger, Lunge },
                { InputHelper.Action.LeftShoulder, Block },
                { InputHelper.Action.LeftTrigger, ThrowShield }
            };
        }

        public override void OnRightThumbstick(Vector2 value)
        {
            var angleIsLeft = Util.IsAngleLeft(Math.Atan2(value.Y, value.X));

            if (value.X != 0 || value.Y != 0)
                Console.WriteLine("sup");

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
            Stage.AddAttack(new KnightSwordSwing(this));
        }

        private void Lunge()
        {
            Console.WriteLine("lunge");
        }

        private void Block()
        {
            Console.WriteLine("block");
        }

        private void ThrowShield()
        {
            Console.WriteLine("throw shield");
        }
    }
}
