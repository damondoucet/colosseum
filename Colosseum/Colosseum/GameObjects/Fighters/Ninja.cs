using Colosseum.GameObjects.Attacks;
using Colosseum.GameObjects.Attacks.Melee;
using Colosseum.GameObjects.Attacks.Projectiles;
using Colosseum.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Fighters
{
    class Ninja : Fighter, Thruster
    {
        private string _weaponAsset;

        private bool _isAttacking;

        private double _timeLeftInvisible;
        private bool _cloneInUse;

        private bool _bombInUse;

        private double _counterTimeLeft;

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
            _cloneInUse = false;
            _bombInUse = false;

            _counterTimeLeft = 0;

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

            if (_counterTimeLeft > 0)
            {
                _counterTimeLeft -= gameTime.ElapsedGameTime.TotalSeconds;

                if (_counterTimeLeft < 0)
                    Cooldown += Constants.Fighters.Ninja.Abilities.Counter.Cooldown;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            if (_timeLeftInvisible <= 0)
                base.Draw(batch, gameTime);
        }

        private void SpawnClone()
        {
            if (_isAttacking || _cloneInUse || _counterTimeLeft > 0)
                return;

            _timeLeftInvisible = Constants.Fighters.Ninja.Abilities.Clone.InvisibilityTimeLength;
            _cloneInUse = true;
            Stage.AddAttack(new NinjaClone(this, WeaponAngle));
        }

        public void OnCloneFinished()
        {
            _cloneInUse = false;
        }

        private void Thrust()
        {
            if (_isAttacking || _counterTimeLeft > 0)
                return;

            _isAttacking = true;
            _weaponAsset = Constants.Assets.Ninja.Thrust;
            Stage.AddAttack(ThrustFactory.CreateNinjaThrust(this));
        }

        public void OnThrustFinished()
        {
            _isAttacking = false;
            _weaponAsset = Constants.Assets.Ninja.Weapon;
            Cooldown = Constants.Fighters.Ninja.Abilities.Thrust.Cooldown;
        }

        private void Counter()
        {
            if (_isAttacking || _counterTimeLeft > 0)
                return;

            // TODO: render differently if countering
            _counterTimeLeft = Constants.Fighters.Ninja.Abilities.Counter.Duration;
        }

        private void DropBomb()
        {
            if (_isAttacking || _bombInUse || _counterTimeLeft > 0)
                return;

            _bombInUse = true;
            Stage.AddAttack(new NinjaBomb(this));
        }

        public void OnBombExploding()
        {
            _bombInUse = false;
        }

        public override void Stun(Attack source, double time)
        {
            if (_counterTimeLeft > 0 && source is Projectile)  // yuck
                PerformCounter(source);
            else
                base.Stun(source, time);
        }

        public override void OnHit(Attack attack)
        {
            if (_counterTimeLeft > 0 && attack is Projectile)  // yuck...
                PerformCounter(attack);
            else
                base.OnHit(attack);
        }

        private void PerformCounter(Attack attack)
        {
            var vector = Constants.Fighters.Ninja.Abilities.Counter.Radius * Util.VectorFromAngle(WeaponAngle);

            TopLeftPosition = attack.Source.TopLeftPosition + vector;
            _counterTimeLeft = 0;  // note we don't go on cooldown because of this
        }

        // xxx
        // override onHit, give it attack parameter
        // if attack is projectile (yuck) then tp to attacker
        // attack should have source 
        // else base.onHit()
    }
}
