using Colosseum.GameObjects.Attacks.Melee;
using Colosseum.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Fighters
{
    class Ninja : Fighter
    {
        private string _weaponAsset;

        private bool _isAttacking;

        private double _timeLeftInvisible;
        private bool _hasClone;

        public override string HeadAsset { get { return Constants.Assets.Ninja.Head; } }
        public override string BodyAsset { get { return Constants.Assets.Ninja.Body; } }
        public override string WeaponAsset { get { return _weaponAsset; } }

        protected override float DashVelocity { get { return Constants.Fighters.Ninja.DashVelocity; } } 
        protected override float TotalDashTime { get { return Constants.Fighters.Ninja.DashTime; } }
        protected override float DashCooldown { get { return Constants.Fighters.Ninja.DashCooldown; } }

        private readonly Dictionary<FighterInputDispatcher.Action, Action> _buttonToAbility;
        protected override Dictionary<FighterInputDispatcher.Action, Action> ButtonToAbility { get { return _buttonToAbility; } }

        public Ninja(Stage stage, Vector2 topLeftPosition, float weaponAngle)
            : base(stage, topLeftPosition, weaponAngle)
        {
            _weaponAsset = Constants.Assets.Ninja.Weapon;
            _isAttacking = false;
            _hasClone = false;

            _buttonToAbility = new Dictionary<FighterInputDispatcher.Action, Action>()
            {
                { FighterInputDispatcher.Action.LeftShoulder, SpawnClone },
                { FighterInputDispatcher.Action.LeftTrigger, Counter },
                { FighterInputDispatcher.Action.RightShoulder, Thrust },
                { FighterInputDispatcher.Action.RightTrigger, DropBomb },
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (_timeLeftInvisible > 0)
                _timeLeftInvisible -= gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            if (_timeLeftInvisible <= 0)
                base.Draw(batch, gameTime);
        }

        private void SpawnClone()
        {
            if (_hasClone)
                return;

            _timeLeftInvisible = Constants.Fighters.Ninja.Abilities.Clone.InvisibilityTimeLength;
            _hasClone = true;
            Stage.AddAttack(new NinjaClone(this, WeaponAngle));
        }

        public void OnCloneFinished()
        {
            _hasClone = false;
        }

        private void Thrust()
        {
            if (_isAttacking)
                return;

            _isAttacking = true;
            _weaponAsset = Constants.Assets.Ninja.Thrust;
            Stage.AddAttack(new NinjaThrust(this));
        }

        public void OnThrustFinished()
        {
            _isAttacking = false;
            _weaponAsset = Constants.Assets.Ninja.Weapon;
            Cooldown = Constants.Fighters.Ninja.Abilities.Thrust.Cooldown;
        }

        private void Counter()
        { 
            
        }

        private void DropBomb()
        { 
            
        }
    }
}
