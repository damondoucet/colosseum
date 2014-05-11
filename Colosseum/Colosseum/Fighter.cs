using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Colosseum
{
    class Fighter : MoveableGameObject
    {
        private static List<string> FighterAssetNames = new List<string> 
        {
            Constants.Assets.FighterBody, Constants.Assets.FighterHead, Constants.Assets.FighterWeapon 
        };

        public override int Width { get { return 64; } }
        public override int Height { get { return 64; } }

        public override bool IgnoresPlatforms { get { return false; } }
        public override bool IgnoresBounds { get { return false; } }
        public override bool IgnoresGravity { get { return false; } }

        public float WeaponAngle { get; set; }

        private float _dashAngle;
        public double _dashTimeLeft;

        public double Cooldown { get; set; }

        public Fighter(Stage stage, Vector2 position, float weaponAngle)
            : base(stage, position, FighterAssetNames)
        {
            Velocity = Vector2.Zero;
            WeaponAngle = weaponAngle;

            _dashAngle = 0;
            _dashTimeLeft = 0;

            Cooldown = 0;
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

        protected override Dictionary<string, Vector2> ComputeAssetNameToOffset()
        {
            var bodySize = FindTextureSize(Constants.Assets.FighterBody);
            var headSize = FindTextureSize(Constants.Assets.FighterHead);
            var weaponSize = FindTextureSize(Constants.Assets.FighterWeapon);

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

            if (_dashTimeLeft > 0)
            {
                _dashTimeLeft -= gameTime.ElapsedGameTime.TotalSeconds;

                if (_dashTimeLeft <= 0)
                {
                    Velocity -= ComputeDashVelocityVector();
                    Cooldown = Constants.FighterDashCooldown;
                }
            }
            else if (Cooldown > 0)
                Cooldown -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        private bool CanPerformAction()
        {
            return Cooldown <= double.Epsilon && _dashTimeLeft <= double.Epsilon;
        }

        public void HandleAction(InputHelper.Action action, Vector2 rightThumbstick)
        {
            if (!CanPerformAction())
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
                    _dashAngle = WeaponAngle;
                    _dashTimeLeft = Constants.FighterDashTime;
                    Velocity += ComputeDashVelocityVector();
                    break;
                default:
                    Console.WriteLine("Invalid action: {0}. Ignoring", action);
                    return;
            }
        }

        public void OnLeftThumbstick(Vector2 value)
        {
            if (!CanPerformAction())
                return;

            var x = value.X;
            var y = value.Y;

            // TODO(ddoucet): walk slower depending on value of vector
            if (x < 0)
                HandleAction(InputHelper.Action.Left, Vector2.Zero);
            else if (x > 0)
                HandleAction(InputHelper.Action.Right, Vector2.Zero);

            if (y > 0)
                HandleAction(InputHelper.Action.Jump, Vector2.Zero);
        }

        public void OnRightThumbstick(Vector2 value)
        {
            if (value.X != 0 || value.Y != 0)
                WeaponAngle = (float)Math.Atan2(-value.Y, value.X);
        }

        private Vector2 ComputeDashVelocityVector()
        {
            var vector = Constants.FighterDashVelocity * Util.VectorFromAngle(_dashAngle);

            if (IsSittingOnPlatform())
                vector.Y = Math.Min(0, vector.Y);

            return vector;
        }
    }
}
