using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Melee
{
    class KnightThrust : TimedMeleeAttack
    {
        private readonly Knight _knight;

        protected override double PhaseInTime { get { return Constants.Fighters.Knight.Abilities.Thrust.PhaseInTime; } }
        protected override double TimeToLive { get { return Constants.Fighters.Knight.Abilities.Thrust.TimeToLive; } }

        public override int Width { get { return Constants.Fighters.Knight.Abilities.Thrust.Width; } }
        public override int Height { get { return Constants.Fighters.Knight.Abilities.Thrust.Height; } }

        public KnightThrust(Knight knight)
            : base(knight.Stage)
        {
            _knight = knight;
        }

        public override void ExitStage()
        {
            _knight.OnThrustFinished();
            base.ExitStage();
        }

        public override Collideable ComputeCollideable()
        {
            return _knight.ComputeWeaponCollideable();
        }
    }
}
