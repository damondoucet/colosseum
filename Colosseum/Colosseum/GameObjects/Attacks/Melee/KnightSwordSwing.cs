using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Melee
{
    class KnightSwordSwing : MeleeAttack
    {
        public override int Width { get { return (int)TextureDictionary.FindTextureSize(_knight.GetWeaponAsset()).X; } }
        public override int Height { get { return (int)TextureDictionary.FindTextureSize(_knight.GetWeaponAsset()).Y; } }

        // sign of the angle change depending on side knight is facing
        private const int LeftSign = 1;
        private const int RightSign = -1;

        private readonly Knight _knight;

        public KnightSwordSwing(Knight knight)
            : base(knight.Stage, knight.ComputeWeaponOffset())
        {
            _knight = knight;
            _knight.WeaponAngle = ComputeStartAngle();
        }

        protected override List<Asset> ComputeAssets()
        {
            // don't render; let the knight take care of it for us
            return new List<Asset>()
            {
            };
        }

        public override Collideable ComputeCollideable()
        {
            // weapon is rotated a specific angle; to go from the original top left corner to the center
            // you need to go rotate a little more
            // although honestly I'm not really sure why it's pi/2 in this case
            var angle = _knight.WeaponAngle + Math.PI / 2;
            var angleVector = Util.VectorFromAngle(angle);

            var weaponSize = TextureDictionary.FindTextureSize(_knight.GetWeaponAsset());
            var center = _knight.TopLeftPosition + _knight.ComputeWeaponOffset() + angleVector * weaponSize / 2.0f;

            return new Rect(center, weaponSize.X, weaponSize.Y, angle);
        }

        private int ComputeAngleChangeSign()
        {
            return _knight.IsFacingLeft() ? LeftSign : RightSign;
        }

        private float ComputeStartAngle()
        {
            return (float)(Math.PI / 2 +
                ComputeAngleChangeSign() * Constants.Fighters.Knight.Abilities.SwordSwing.StartAngle);
        }

        private float ComputeEndAngle()
        {
            return (float)(Math.PI / 2 +
                ComputeAngleChangeSign() * Constants.Fighters.Knight.Abilities.SwordSwing.EndAngle);
        }

        protected override bool ShouldExit()
        {
            return Math.Sin(_knight.WeaponAngle) >= Math.Sin(ComputeEndAngle());
        }

        private float ComputeNextAngle(GameTime gameTime)
        {
            var currentAngle = _knight.WeaponAngle;
            var sign = ComputeAngleChangeSign();

            var angularVelocity = Constants.Fighters.Knight.Abilities.SwordSwing.AngularVelocity;

            return (float)(currentAngle + sign * angularVelocity * gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void ExitStage()
        {
            _knight.OnSwingSwordFinished();
            base.ExitStage();
        }

        public override void Update(GameTime gameTime)
        {
            _knight.WeaponAngle = ComputeNextAngle(gameTime);

            base.Update(gameTime);
        }
    }
}
