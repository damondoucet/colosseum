using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Melee
{
    class NinjaClone : TimedMeleeAttack
    {
        private readonly Ninja _ninja;

        private double _dashTimeLeft;

        protected override bool ShouldDraw { get { return true; } }

        protected override double PhaseInTime { get { return 0; } }
        protected override double TimeToLive { get { return Constants.Fighters.Ninja.Abilities.Clone.CloneLifetime; } }

        public override int Width { get { return Constants.Fighters.Width; } }
        public override int Height { get { return Constants.Fighters.Width; } }

        public override bool IgnoresBounds { get { return false; } }
        public override bool IgnoresGravity { get { return false; } }
        public override bool IgnoresPlatforms { get { return false; } }

        public NinjaClone(Ninja ninja, double angle)
            : base(ninja.Stage)
        {
            _ninja = ninja;
            _dashTimeLeft = Constants.Fighters.Ninja.DashTime;
            TopLeftPosition = _ninja.TopLeftPosition;
            Velocity = Constants.Fighters.Ninja.DashVelocity * Util.VectorFromAngle(angle);
        }

        public override void Update(GameTime gameTime)
        {
            if (_dashTimeLeft > 0)
            {
                _dashTimeLeft -= gameTime.ElapsedGameTime.TotalSeconds;

                if (_dashTimeLeft < 0)
                    Velocity = Vector2.Zero;
            }

            base.Update(gameTime);
        }

        public override void ExitStage()
        {
            _ninja.OnCloneFinished();
            base.ExitStage();
        }

        protected override List<Asset> ComputeAssets()
        {
            return new FighterAssetComputer().ComputeAssets(_ninja, TopLeftPosition);   
        }

        public override Collideable ComputeCollideable()
        {
            // TODO: needs to be changed if clone will swing sword
            return new NonCollideable();
        }
    }
}
