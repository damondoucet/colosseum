using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class KnightShield : Projectile
    {
        public enum State
        { 
            Flying,
            Sitting,
            Shielding,
            Stored
        }

        private static Dictionary<State, Vector2> StateToAssetSize = new Dictionary<State, Vector2>()
        {
            { State.Flying, new Vector2(Constants.Fighters.Knight.Abilities.Shield.FlyingAssetWidth, Constants.Fighters.Knight.Abilities.Shield.FlyingAssetHeight) },
            { State.Sitting, new Vector2(Constants.Fighters.Knight.Abilities.Shield.SittingAssetWidth, Constants.Fighters.Knight.Abilities.Shield.SittingAssetHeight) },
            { State.Shielding, new Vector2(Constants.Fighters.Knight.Abilities.Shield.ShieldingAssetWidth, Constants.Fighters.Knight.Abilities.Shield.ShieldingAssetHeight) },
            { State.Stored, Vector2.Zero },
        };

        public override int Width { get { return (int)StateToAssetSize[CurrentState].X; } }
        public override int Height { get { return (int)StateToAssetSize[CurrentState].Y; } }

        private bool ActsLikeProjectile { get { return CurrentState == State.Flying || CurrentState == State.Sitting; } }

        public override bool IgnoresBounds { get { return !ActsLikeProjectile; } }
        public override bool IgnoresGravity { get { return CurrentState != State.Sitting; } }
        public override bool IgnoresPlatforms { get { return !ActsLikeProjectile; } }

        // the shield is pretty weird because it's not _really_ a projectile by these definitions
        // but we need it to be one for counter since I don't have time to do this the right way :/
        protected override double PhaseInTime { get { return 0; } }
        protected override double TimeToLive { get { return 0; } }

        private readonly Knight _knight;
        public State CurrentState { get; set; }

        public KnightShield(Knight knight)
            : base(knight, Vector2.Zero, Vector2.Zero)
        {
            _knight = knight;
            CurrentState = State.Stored;
        }

        public void Throw(double angle)
        {
            TopLeftPosition = ComputeTopLeft();
            Velocity = Constants.Fighters.Knight.Abilities.Shield.ShieldThrowVelocity * Util.VectorFromAngle(_knight.WeaponAngle);

            if (Math.Abs(Velocity.Y) < Constants.YSensitivity)
                Velocity.Y = 0;

            CurrentState = State.Flying;

            if (_knight.IsSittingOnPlatform() && TopLeftPosition.Y > _knight.TopLeftPosition.Y + _knight.Height)
            {
                TopLeftPosition.Y = _knight.TopLeftPosition.Y + _knight.Height;
                Velocity.Y = 0;
            }

            Stage.AddAttack(this);
        }

        private Vector2 ComputeTopLeft()
        {
            var knightCenter = _knight.ComputeCenter();

            var r = Constants.Fighters.Knight.Abilities.Shield.DistanceFromBodyCenter;
            var apex = knightCenter + r * Util.VectorFromAngle(_knight.WeaponAngle);

            var assetSize = TextureDictionary.FindTextureSize(Constants.GameAssets.Knight.ShieldFlying);

            return apex - assetSize / 2;
        }

        protected override bool ShouldExit()
        {
            // managed by other methods
            return false;
        }

        public override bool HasCollisionWithFighter(Fighter fighter)
        {
            return ActsLikeProjectile && base.HasCollisionWithFighter(fighter);
        }

        public override bool HasCollisionWithAttack(Attack attack)
        {
            return CurrentState == State.Shielding &&
                attack is Projectile &&  // bad...
                ComputeCollideable().HasCollision(attack.ComputeCollideable());
        }

        public override void OnAttackCollision(Attack attack)
        {
            // because we have a shitty collision system, we assume that the contact point is on
            // the line defined by the knight center and the attack center

            // that makes that vector also the normal to the shield
            // the attack's velocity is reflected across this normal

            // for the math: http://math.stackexchange.com/questions/13261/how-to-get-a-reflection-vector
            var normal = (attack.ComputeCenter() - _knight.ComputeCenter()).Norm();

            var v = attack.Velocity;

            attack.Velocity = v - (2 * v.Dot(normal)) * normal;

            ((Projectile)attack).HasBeenReflected = true;
        }

        public override void OnFighterCollision(Fighter fighter)
        {
            if (fighter == _knight)
            {
                if (HasBeenReflected)
                    _knight.Stun(this, Constants.Fighters.Knight.Abilities.Shield.StunLength);

                CurrentState = State.Stored;
                ExitStage();

                Velocity = Vector2.Zero;
            }
            else if (CurrentState == State.Flying)
            {
                fighter.Stun(this, Constants.Fighters.Knight.Abilities.Shield.StunLength);

                CurrentState = State.Sitting;
                Velocity = Vector2.Zero;

                TopLeftPosition -= (StateToAssetSize[State.Sitting] - StateToAssetSize[State.Flying]);
            }
        }

        public override void OnPlatformCollision(Vector2 contactVector)
        {
            if (CurrentState != State.Flying)
                return;

            if (contactVector.Y > 0)
                Velocity.Y = 0;

            CurrentState = State.Sitting;
            TopLeftPosition -= (StateToAssetSize[State.Sitting] - StateToAssetSize[State.Flying]);
            base.OnPlatformCollision(contactVector);
        }

        public override Collideable ComputeCollideable()
        {
            // yuck
            switch (CurrentState)
            { 
                case State.Stored:
                    return new NonCollideable();
                case State.Shielding:
                    return ComputeShieldingCollideable();
                case State.Flying:
                    var angle = Math.Atan2(-Velocity.Y, Velocity.X);
                    var flyingAssetSize = TextureDictionary.FindTextureSize(Constants.GameAssets.Knight.ShieldFlying);
                    var center = TopLeftPosition + flyingAssetSize / 2;

                    return new Rect(center, flyingAssetSize.X, flyingAssetSize.Y, angle);
                case State.Sitting:
                    var sittingAssetSize = TextureDictionary.FindTextureSize(Constants.GameAssets.Knight.ShieldSitting);
                    return new Rect(TopLeftPosition + sittingAssetSize / 2, sittingAssetSize.X, sittingAssetSize.Y, 0);
                default:
                    throw new Exception("Invalid shield state " + CurrentState);
            }
        }

        private Collideable ComputeShieldingCollideable()
        {
            var knightCenter = _knight.ComputeCenter();

            // see Constants file for description of collision approximation
            var r = Constants.Fighters.Knight.Abilities.Shield.DistanceFromBodyCenter;
            var width = Constants.Fighters.Knight.Abilities.Shield.CollisionRectangleWidth;
            var height = Constants.Fighters.Knight.Abilities.Shield.CollisionRectangleHeight;

            var rectCenter = knightCenter + (float)(r - height / 2) * Util.VectorFromAngle(_knight.WeaponAngle);

            return new Rect(rectCenter, (float)width, (float)height, _knight.WeaponAngle + MathHelper.PiOver2);
        }

        protected override List<Asset> ComputeAssets()
        {
            // double yuck
            switch (CurrentState)
            {
                case State.Stored:
                    return new List<Asset>();
                case State.Shielding:
                    var knightCenter = _knight.ComputeCenter();
                    
                    var r = Constants.Fighters.Knight.Abilities.Shield.DistanceFromBodyCenter;
                    var apex = knightCenter + r * Util.VectorFromAngle(_knight.WeaponAngle);

                    var assetSize = TextureDictionary.FindTextureSize(Constants.GameAssets.Knight.Shielding);
                    
                    var topLeft = apex - assetSize.X / 2 * Util.VectorFromAngle(_knight.WeaponAngle) - assetSize / 2;

                    return new Asset(Stage, Constants.GameAssets.Knight.Shielding, topLeft, (float)_knight.WeaponAngle).SingleToList();
                case State.Flying:
                    var angle = Math.Atan2(-Velocity.Y, Velocity.X);
                    return new Asset(Stage, Constants.GameAssets.Knight.ShieldFlying, TopLeftPosition, (float)angle).SingleToList();
                case State.Sitting:
                    return new Asset(Stage, Constants.GameAssets.Knight.ShieldSitting, TopLeftPosition).SingleToList();
                default:
                    throw new Exception("Invalid shield state " + CurrentState);
            }
        }
    }
}
