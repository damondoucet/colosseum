using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using System;

namespace Colosseum.GameObjects.Attacks.Melee
{
    class KnightSwordSwing : MeleeAttack
    {
        private readonly int _swordWidth;
        private readonly int _swordHeight;

        public override int Width { get { return _swordWidth; } }
        public override int Height { get { return _swordHeight; } }

        // sign of the angle change depending on side knight is facing
        private const int LeftSign = 1;
        private const int RightSign = -1;

        private readonly Knight _knight;

        public KnightSwordSwing(Knight knight, int swordWidth, int swordHeight)
            : base(knight)
        {
            _knight = knight;
            _knight.WeaponAngle = ComputeStartAngle();

            _swordWidth = swordWidth;
            _swordHeight = swordHeight;
        }

        public override Collideable ComputeCollideable()
        {
            return _knight.ComputeWeaponCollideable();
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
