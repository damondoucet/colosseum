using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

        protected override string HeadAsset { get { return Constants.Assets.FighterHead; } }
        protected override string BodyAsset { get { return Constants.Assets.FighterBody; } }
        protected override string WeaponAsset { get { return Constants.Assets.FighterWeapon; } }

        protected override float DashVelocity { get { return Constants.Fighters.Knight.DashVelocity; } }
        protected override float TotalDashTime { get { return Constants.Fighters.Knight.DashTime; } }

        private readonly Dictionary<InputHelper.Action, Action> _buttonToAbility;
        protected override Dictionary<InputHelper.Action, Action> ButtonToAbility { get { return _buttonToAbility; } }

        public Knight(Stage stage, Vector2 position, float weaponAngle)
            : base(stage, position, weaponAngle, KnightAssetNames)
        {
            _buttonToAbility = new Dictionary<InputHelper.Action, Action>()
            {
                { InputHelper.Action.RightShoulder, SwingSword },
                { InputHelper.Action.RightTrigger, Lunge },
                { InputHelper.Action.LeftShoulder, Block },
                { InputHelper.Action.LeftTrigger, ThrowShield }
            };
        }

        private void SwingSword()
        {
            Console.WriteLine("swing sword");
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
