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

        public Wizard(Stage stage, Vector2 topLeftPosition, float weaponAngle)
            : base(stage, topLeftPosition, weaponAngle)
        {
            _buttonToAbility = new Dictionary<FighterInputDispatcher.Action, Action>()
            {
                { FighterInputDispatcher.Action.LeftShoulder, SpawnOrDetonateBomb },
                { FighterInputDispatcher.Action.RightShoulder, FireTriangle },
                { FighterInputDispatcher.Action.LeftTrigger, SpawnCloud },
                { FighterInputDispatcher.Action.RightTrigger, ForcePulse }
            };
        }

        private void SpawnOrDetonateBomb()
        { 
            
        }

        private void FireTriangle()
        { 
            
        }

        private void SpawnCloud()
        { 
            
        }

        private void ForcePulse()
        { 
            
        }
    }
}
