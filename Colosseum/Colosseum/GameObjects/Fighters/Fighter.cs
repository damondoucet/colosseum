using Colosseum.GameObjects.Attacks;
using Colosseum.GameObjects.Attacks.Projectiles;
using Colosseum.GameObjects.Collisions;
using Colosseum.Graphics;
using Colosseum.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Fighters
{
    abstract class Fighter : MoveableGameObject
    {
        public abstract string StandardHeadAsset { get; }
        public abstract string StandardBodyAsset { get; }
        public abstract string StandardWeaponAsset { get; }

        public abstract string StunnedHeadAsset { get; }

        public string HeadAsset { get; set; }
        public string BodyAsset { get; set; }
        public string WeaponAsset { get; set; }

        protected abstract float DashVelocity { get; }
        protected abstract float TotalDashTime { get; }
        protected abstract float DashCooldown { get; }

        protected abstract Dictionary<FighterInputDispatcher.Action, Action> ButtonToAbility { get; }

        public override int Width { get { return Constants.Fighters.Width; } }
        public override int Height { get { return Constants.Fighters.Height; } }

        public override bool IgnoresPlatforms { get { return false; } }
        public override bool IgnoresBounds { get { return false; } }
        public override bool IgnoresGravity { get { return false; } }

        private readonly int _playerIndex;  // 0 or 1

        public float WeaponAngle { get; set; }

        private Vector2 _dashVelocityVector;
        private bool _canDash;

        private float _dashAngle;
        public double _dashTimeLeft;

        protected double Cooldown;
        public double ShieldCooldown { get; set; }

        private double _secondsSinceLastBlink;

        private bool _isStunned;

        private double _slowDur;

        public Fighter(Stage stage, int playerIndex, Vector2 position, float weaponAngle)
            : base(stage, position)
        {
            _playerIndex = playerIndex;

            Velocity = Vector2.Zero;
            WeaponAngle = weaponAngle;

            HeadAsset = StandardHeadAsset;
            BodyAsset = StandardBodyAsset;
            WeaponAsset = StandardWeaponAsset;

            _dashAngle = 0;
            _dashTimeLeft = 0;

            Cooldown = 0;

            ShieldCooldown = 0;
            _secondsSinceLastBlink = 0;

            _canDash = true;
            _isStunned = false;

            _slowDur = 0;
        }

        public bool IsFacingLeft()
        {
            return Util.IsAngleLeft(WeaponAngle);
        }

        public Vector2 ComputeWeaponOffset()
        {
            var bodySize = TextureDictionary.FindTextureSize(BodyAsset);
            var weaponSize = TextureDictionary.FindTextureSize(WeaponAsset);

            var weaponOrbitRadius = bodySize.X / 2.0f + Constants.Fighters.WeaponDistance + weaponSize.X / 2.0f;

            return bodySize / 2.0f + weaponOrbitRadius * Util.VectorFromAngle(WeaponAngle);
        }

        public bool ShouldBlink()
        {
            return ShieldCooldown >= 0 && _secondsSinceLastBlink < Constants.Fighters.BlinkLength;
        }

        public virtual Color GetTint()
        {
            return ShouldBlink()
                ? Constants.Fighters.BlinkTint : Color.White;
        }

        protected override List<Asset> ComputeAssets()
        {
            return new FighterAssetComputer().ComputeAssets(this, TopLeftPosition, GetTint());
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var dt = gameTime.ElapsedGameTime.TotalSeconds;

            if (_dashTimeLeft > 0)
                UpdateDash(dt);
            else if (Cooldown > 0)
                Cooldown -= dt;

            if (Cooldown <= 0 && _isStunned)
            {
                HeadAsset = StandardHeadAsset;
                _isStunned = false;
            }

            if (_slowDur > 0)
                _slowDur -= dt;

            CheckShield(dt);
        }

        public virtual void Stun(Attack source, double time)
        {
            Cooldown = Math.Max(Cooldown, time);
            
            _isStunned = true;
            HeadAsset = StunnedHeadAsset;
        }

        public virtual void Slow(Attack source, double time)
        {
            _slowDur = time;

        }

        private void UpdateDash(double deltaTime)
        {
            _dashTimeLeft -= deltaTime;

            if (_dashTimeLeft <= 0)
            {
                Velocity -= _dashVelocityVector;
                Cooldown = DashCooldown;
            }
        }

        private void CheckShield(double deltaTime)
        {
            if (ShieldCooldown >= 0)
            {
                ShieldCooldown -= deltaTime;

                if (_secondsSinceLastBlink > Constants.Fighters.BlinkPeriod)
                    _secondsSinceLastBlink -= Constants.Fighters.BlinkPeriod;

                _secondsSinceLastBlink += deltaTime;
            }
        }

        // CanMove assumed to be a subset of CanPerformAction
        protected virtual bool CanMove()
        {
            return _dashTimeLeft <= 0 && !_isStunned;
        }

        protected virtual bool CanPerformAction()
        {
            return Cooldown <= 0;
        }

        public virtual void HandleAction(FighterInputDispatcher.Action action, bool pressed, Vector2 leftThumbstick, Vector2 rightThumbstick)
        {
            if (!pressed)  // sometimes usedful for child classes to know when a button is released, e.g. knight shielding
                return;

            if (!CanMove())
                return;

            switch (action)
            {
                case FighterInputDispatcher.Action.Jump:
                    if (IsSittingOnPlatform() && Velocity.Y == 0)
                        Velocity += new Vector2(0, -Constants.Fighters.JumpVelocity);
                    break;
                case FighterInputDispatcher.Action.Left:
                case FighterInputDispatcher.Action.Right:
                    var magnitude = Constants.Fighters.XMovement * leftThumbstick.X;

                    if (_slowDur > 0)
                        magnitude *= Constants.Fighters.SlowStrength;

                    TopLeftPosition += new Vector2(magnitude, 0);
                    break;
                case FighterInputDispatcher.Action.Dash:
                    if (!_canDash || Cooldown > 0)
                        break;

                    _dashAngle = WeaponAngle;
                    _dashTimeLeft = TotalDashTime;
                    _dashVelocityVector = ComputeDashVelocityVector(leftThumbstick);
                    Velocity += _dashVelocityVector;

                    _canDash = false;  // will reset when landing on a platform
                    break;
                case FighterInputDispatcher.Action.LeftShoulder:
                case FighterInputDispatcher.Action.LeftTrigger:
                case FighterInputDispatcher.Action.RightShoulder:
                case FighterInputDispatcher.Action.RightTrigger:
                    if (CanPerformAction())
                        ButtonToAbility[action]();
                    break;
                default:
                    Console.WriteLine("Invalid action: {0}. Ignoring", action);
                    return;
            }
        }

        public void OnLeftThumbstick(Vector2 value)
        {
            if (_dashTimeLeft > 0 || _isStunned)
                return;

            var x = value.X;
            var y = value.Y;

            if (x < 0)
                HandleAction(FighterInputDispatcher.Action.Left, true, value, Vector2.Zero);
            else if (x > 0)
                HandleAction(FighterInputDispatcher.Action.Right, true, value, Vector2.Zero);

            if (y > Constants.Fighters.JumpSensitivity)
                HandleAction(FighterInputDispatcher.Action.Jump, true, value, Vector2.Zero);
        }
        
        public virtual void OnRightThumbstick(Vector2 value)
        {
            if (value.X != 0 || value.Y != 0)
                WeaponAngle = (float)Math.Atan2(-value.Y, value.X);
        }

        private Vector2 ComputeDashVelocityVector(Vector2 leftThumbstick)
        {
            var angle = leftThumbstick.LengthSquared() > Constants.ThumbstickSensitivity
                ? Math.Atan2(-leftThumbstick.Y, leftThumbstick.X)
                : IsFacingLeft() ? Math.PI : 0;

            var angleVector = Util.VectorFromAngle(angle);
            
            var vector = DashVelocity * angleVector;

            if (IsSittingOnPlatform())
            {
                if (Math.Abs(vector.Y) < Constants.ThumbstickSensitivity)
                    vector.Y = 0;
                vector.Y = Math.Min(0, vector.Y);
            }

            return vector;
        }

        public Collideable ComputeCollideable()
        {
            return ComputeCollideable(TopLeftPosition);
        }

        public Collideable ComputeCollideable(Vector2 topLeftPosition)
        {
            var headSize = TextureDictionary.FindTextureSize(HeadAsset);
            return new CompoundCollideable(
                new Collideable[]
                {
                    new Circle(topLeftPosition + new Vector2(Width / 2.0f, Height / 2.0f), Width / 2.0f),  // body
                    new Circle(topLeftPosition + new Vector2(Width / 2.0f, -headSize.Y / 2.0f), headSize.X / 2.0f)  // head
                    // we don't include the weapon in the hitbox
                });   
        }
        
        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);

            // TODO(ddoucet): technically this is unnecessary (should only compute collideable if necessary)
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable(), ComputeCenter());
        }

        public virtual void OnHit(Attack attack)
        {
            if (ShieldCooldown > 0)
                Stage.Winner = _playerIndex ^ 1;
            else
            {
                ShieldCooldown = Constants.Fighters.ShieldCooldown;
                _secondsSinceLastBlink = 0;
            }
        }

        public override void OnPlatformCollision(Vector2 contactVector)
        {
            if (contactVector.Y > 0)
                _canDash = true;

            if (Math.Abs(contactVector.X) > float.Epsilon)  // x != 0
            {
                Velocity.X = 0;
                _dashVelocityVector = new Vector2(0, _dashVelocityVector.Y);
            }
            if (Math.Abs(contactVector.Y) > float.Epsilon)  // y != 0
            {
                Velocity.Y = 0;
                _dashVelocityVector = new Vector2(_dashVelocityVector.X, 0);
            }
        }
    }
}
