using Colosseum.GameObjects.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        protected abstract Dictionary<InputHelper.Action, Action> ButtonToAbility { get; }

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

        private double _cooldown;
        private double _shieldCooldown;

        private double _secondsSinceLastBlink;

        public Fighter(Stage stage, Vector2 position, float weaponAngle, List<string> assetNames)
            : base(stage, position, assetNames)
        {
            Velocity = Vector2.Zero;
            WeaponAngle = weaponAngle;

            _dashAngle = 0;
            _dashTimeLeft = 0;

            _cooldown = 0;

            _shieldCooldown = 0;
            _secondsSinceLastBlink = 0;

            _canDash = true;
        }

        private bool IsFacingLeft()
        {
            return Math.Cos(WeaponAngle) < 0;  // heh...
        }

        public override float GetAssetRotation(string assetName)
        {
            return assetName == Constants.Assets.FighterWeapon ? WeaponAngle : 0;
        }

        public override SpriteEffects GetAssetSpriteEffects(string assetName)
        {
            return IsFacingLeft() && (assetName == Constants.Assets.FighterBody || assetName == Constants.Assets.FighterHead) ?
                SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        public override Color GetAssetTint(string assetName)
        {
            return _shieldCooldown >= 0 && _secondsSinceLastBlink < Constants.BlinkLength
                ? Constants.BlinkTint : Color.White;
        }

        protected override Dictionary<string, Vector2> ComputeAssetNameToOffset()
        {
            var bodySize = TextureDictionary.FindTextureSize(Constants.Assets.FighterBody);
            var headSize = TextureDictionary.FindTextureSize(Constants.Assets.FighterHead);
            var weaponSize = TextureDictionary.FindTextureSize(Constants.Assets.FighterWeapon);

            var weaponOrbitRadius = bodySize.X / 2.0f + Constants.FighterWeaponDistance + weaponSize.X / 2.0f;

            return new Dictionary<string, Vector2>()
            {
                { Constants.Assets.FighterBody, bodySize / 2.0f },
                { Constants.Assets.FighterHead, new Vector2(bodySize.X / 2, -headSize.Y / 2) },
                { Constants.Assets.FighterWeapon, 
                    bodySize / 2.0f + weaponOrbitRadius * Util.VectorFromAngle(WeaponAngle) }
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var dt = gameTime.ElapsedGameTime.TotalSeconds;

            if (_dashTimeLeft > 0)
                UpdateDash(dt);
            else if (_cooldown > 0)
                _cooldown -= dt;

            CheckShield(dt);
        }

        private void UpdateDash(double deltaTime)
        {
            _dashTimeLeft -= deltaTime;

            if (_dashTimeLeft <= 0)
            {
                Velocity -= _dashVelocityVector;
                _cooldown = Constants.FighterDashCooldown;
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

        private bool CanPerformAction()
        {
            return _cooldown <= double.Epsilon && _dashTimeLeft <= double.Epsilon;
        }

        public void HandleAction(InputHelper.Action action, Vector2 leftThumbstick, Vector2 rightThumbstick)
        {
            if (_dashTimeLeft > 0)
                return;

            switch (action)
            {
                case InputHelper.Action.Jump:
                    if (IsSittingOnPlatform() && Velocity.Y == 0)
                        Velocity += new Vector2(0, -Constants.FighterJumpVelocity);
                    break;
                case InputHelper.Action.Left:
                    TopLeftPosition += new Vector2(-Constants.FighterMovementX, 0);
                    break;
                case InputHelper.Action.Right:
                    TopLeftPosition += new Vector2(Constants.FighterMovementX, 0);
                    break;
                case InputHelper.Action.Dash:
                    if (!_canDash || _cooldown > 0)
                        break;

                    _dashAngle = WeaponAngle;
                    _dashTimeLeft = TotalDashTime;
                    _dashVelocityVector = ComputeDashVelocityVector(leftThumbstick);
                    Velocity += _dashVelocityVector;

                    _canDash = false;  // will reset when landing on a platform
                    break;
                case InputHelper.Action.Projectile:
                    if (_cooldown <= 0)
                        FireProjectile();
                    break;

                case InputHelper.Action.LeftShoulder:
                case InputHelper.Action.LeftTrigger:
                case InputHelper.Action.RightShoulder:
                case InputHelper.Action.RightTrigger:
                    ButtonToAbility[action]();
                    break;
                default:
                    Console.WriteLine("Invalid action: {0}. Ignoring", action);
                    return;
            }
        }

        private void FireProjectile()
        {
            var velocity = Constants.Projectiles.Test.VelocityMagnitude * Util.VectorFromAngle(WeaponAngle);
            var position = ComputeProjectileStartPosition();
            Stage.ProjectileFactory.CreateTestProjectile(position, velocity);
            _cooldown += Constants.Projectiles.Test.Cooldown;
        }

        private Vector2 ComputeProjectileStartPosition()
        {
            var bodyCenter = TopLeftPosition + new Vector2(Width, Height) / 2.0f;

            var weaponSize = TextureDictionary.FindTextureSize(Constants.Assets.FighterWeapon);
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
                HandleAction(InputHelper.Action.Left, value, Vector2.Zero);
            else if (x > 0)
                HandleAction(InputHelper.Action.Right, value, Vector2.Zero);
        }

        public void OnRightThumbstick(Vector2 value)
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
            var headSize = TextureDictionary.FindTextureSize(Constants.Assets.FighterHead);
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

            // TODO: technically this is unnecessary (should only compute collideable if necessary)
            HitboxPainter.MaybePaintHitbox(batch, ComputeCollideable());
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
