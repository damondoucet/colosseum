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
        protected abstract string HeadAsset { get; }
        protected abstract string BodyAsset { get; }
        protected abstract string WeaponAsset { get; }

        protected abstract float DashVelocity { get; }
        protected abstract float TotalDashTime { get; }

        protected abstract Dictionary<FighterInputDispatcher.Action, Action> ButtonToAbility { get; }

        public override int Width { get { return 64; } }
        public override int Height { get { return 64; } }

        public override bool IgnoresPlatforms { get { return false; } }
        public override bool IgnoresBounds { get { return false; } }
        public override bool IgnoresGravity { get { return false; } }

        public float WeaponAngle { get; set; }

        private Vector2 _dashVelocityVector;
        private bool _canDash;

        private float _dashAngle;
        public double _dashTimeLeft;

        protected double Cooldown;
        private double _shieldCooldown;

        private double _secondsSinceLastBlink;

        private bool _isStunned;

        public Fighter(Stage stage, Vector2 position, float weaponAngle)
            : base(stage, position)
        {
            Velocity = Vector2.Zero;
            WeaponAngle = weaponAngle;

            _dashAngle = 0;
            _dashTimeLeft = 0;

            Cooldown = 0;

            _shieldCooldown = 0;
            _secondsSinceLastBlink = 0;

            _canDash = true;
            _isStunned = false;
        }

        public bool IsFacingLeft()
        {
            return Util.IsAngleLeft(WeaponAngle);
        }

        public Vector2 ComputeWeaponOffset()
        {
            var bodySize = TextureDictionary.FindTextureSize(BodyAsset);
            var weaponSize = TextureDictionary.FindTextureSize(WeaponAsset);

            var weaponOrbitRadius = bodySize.X / 2.0f + Constants.FighterWeaponDistance + weaponSize.X / 2.0f;

            return bodySize / 2.0f + weaponOrbitRadius * Util.VectorFromAngle(WeaponAngle);
        }

        protected Color GetTint()
        {
            return _shieldCooldown >= 0 && _secondsSinceLastBlink < Constants.BlinkLength
                ? Constants.BlinkTint : Color.White;
        }

        protected override List<Asset> ComputeAssets()
        {
            // TODO: render differently if stunned
            var bodySize = TextureDictionary.FindTextureSize(BodyAsset);
            var headSize = TextureDictionary.FindTextureSize(HeadAsset);
            var weaponSize = TextureDictionary.FindTextureSize(WeaponAsset);

            var headPosition = TopLeftPosition + new Vector2((bodySize.X - headSize.X) / 2, -headSize.Y);
            var headAngle = WeaponAngle + (Util.IsAngleLeft(WeaponAngle) ? (float)Math.PI : 0);

            var weaponPosition = TopLeftPosition + ComputeWeaponOffset() - weaponSize / 2.0f;

            var tint = GetTint();
            var spriteEffects = IsFacingLeft() ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            return new List<Asset>()
            {
                new Asset(Stage, BodyAsset, TopLeftPosition, 0.0f, tint, spriteEffects),
                new Asset(Stage, HeadAsset, headPosition, headAngle, tint, spriteEffects),

                // we don't flip the weapon since the rotation will handle that for us
                new Asset(Stage, WeaponAsset, weaponPosition, WeaponAngle, tint, SpriteEffects.None)
            };
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

            if (Cooldown <= 0)
                _isStunned = false;

            CheckShield(dt);
        }

        public void Stun(double time)
        {
            Cooldown = Math.Max(Cooldown, time);
            _isStunned = true;
        }

        private void UpdateDash(double deltaTime)
        {
            _dashTimeLeft -= deltaTime;

            if (_dashTimeLeft <= 0)
            {
                Velocity -= _dashVelocityVector;
                Cooldown = Constants.FighterDashCooldown;
            }
        }

        private void CheckShield(double deltaTime)
        {
            if (_shieldCooldown >= 0)
            {
                _shieldCooldown -= deltaTime;

                if (_secondsSinceLastBlink > Constants.BlinkPeriod)
                    _secondsSinceLastBlink -= Constants.BlinkPeriod;

                _secondsSinceLastBlink += deltaTime;
            }
        }

        protected virtual bool CanPerformAction()
        {
            return Cooldown <= 0 && _dashTimeLeft <= 0;
        }

        public virtual void HandleAction(FighterInputDispatcher.Action action, bool pressed, Vector2 leftThumbstick, Vector2 rightThumbstick)
        {
            if (!pressed)  // sometimes usedful for child classes to know when a button is released, e.g. knight shielding
                return;

            if (_dashTimeLeft > 0)
                return;

            switch (action)
            {
                case FighterInputDispatcher.Action.Jump:
                    if (IsSittingOnPlatform() && Velocity.Y == 0)
                        Velocity += new Vector2(0, -Constants.FighterJumpVelocity);
                    break;
                case FighterInputDispatcher.Action.Left:
                case FighterInputDispatcher.Action.Right:
                    TopLeftPosition += new Vector2(Constants.FighterMovementX * leftThumbstick.X, 0);
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
                case FighterInputDispatcher.Action.Projectile:
                    if (Cooldown <= 0)
                        FireProjectile();
                    break;

                case FighterInputDispatcher.Action.LeftShoulder:
                case FighterInputDispatcher.Action.LeftTrigger:
                case FighterInputDispatcher.Action.RightShoulder:
                case FighterInputDispatcher.Action.RightTrigger:
                    if (Cooldown <= 0)
                        ButtonToAbility[action]();
                    break;
                default:
                    Console.WriteLine("Invalid action: {0}. Ignoring", action);
                    return;
            }
        }

        // TODO(ddoucet): remove this when classes are working
        private void FireProjectile()
        {
            var velocity = Constants.Projectiles.Test.VelocityMagnitude * Util.VectorFromAngle(WeaponAngle);
            var position = ComputeProjectileStartPosition();
            Stage.AddAttack(new TestProjectile(Stage, position, velocity));
            Cooldown += Constants.Projectiles.Test.Cooldown;
        }

        private Vector2 ComputeProjectileStartPosition()
        {
            var bodyCenter = TopLeftPosition + new Vector2(Width, Height) / 2.0f;

            var weaponSize = TextureDictionary.FindTextureSize(WeaponAsset);
            var dist = Constants.Projectiles.Test.FireDistance;

            var radius = Width / 2.0f + Constants.FighterWeaponDistance + weaponSize.X + dist;

            return bodyCenter + radius * Util.VectorFromAngle(WeaponAngle) - new Vector2(0, Constants.Projectiles.Test.Height / 2.0f);
        }

        public void OnLeftThumbstick(Vector2 value)
        {
            if (_dashTimeLeft > 0)
                return;

            var x = value.X;
            var y = value.Y;

            // TODO(ddoucet): walk slower depending on value of vector
            if (x < 0)
                HandleAction(FighterInputDispatcher.Action.Left, true, value, Vector2.Zero);
            else if (x > 0)
                HandleAction(FighterInputDispatcher.Action.Right, true, value, Vector2.Zero);
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
            var headSize = TextureDictionary.FindTextureSize(HeadAsset);
            return new CompoundCollideable(
                new Collideable[]
                {
                    new Circle(TopLeftPosition + new Vector2(Width / 2.0f, Height / 2.0f), Width / 2.0f),  // body
                    new Circle(TopLeftPosition + new Vector2(Width / 2.0f, -headSize.Y / 2.0f), headSize.X / 2.0f)  // head
                    // we don't include the weapon in the hitbox
                });
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);

            // TODO(ddoucet): technically this is unnecessary (should only compute collideable if necessary)
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable(), ComputeCenter());
        }

        public void OnHit()
        {
            if (_shieldCooldown > 0)
                Stage.GameOver = true;
            else
            {
                _shieldCooldown = Constants.ShieldCooldown;
                _secondsSinceLastBlink = 0;
            }
        }

        public override void OnPlatformCollision(bool landedOnPlatform)
        {
            if (landedOnPlatform)
                _canDash = true;
            _dashVelocityVector = new Vector2(_dashVelocityVector.X, 0);
        }
    }
}
