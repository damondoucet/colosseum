using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;

namespace Colosseum.GameObjects.Attacks.Melee
{
    class NinjaThrust : TimedMeleeAttack
    {
        private readonly Ninja _ninja;

        protected override double PhaseInTime { get { return Constants.Fighters.Ninja.Abilities.Thrust.PhaseInTime; } }
        protected override double TimeToLive { get { return Constants.Fighters.Ninja.Abilities.Thrust.TimeToLive; } }

        public override int Width { get { return Constants.Fighters.Ninja.Abilities.Thrust.Width; } }
        public override int Height { get { return Constants.Fighters.Ninja.Abilities.Thrust.Height; } }

        public NinjaThrust(Ninja ninja)
            : base(ninja.Stage)
        {
            _ninja = ninja;
        }

        public override void ExitStage()
        {
            _ninja.OnThrustFinished();
            base.ExitStage();
        }

        public override Collideable ComputeCollideable()
        {
            return _ninja.ComputeWeaponCollideable();
        }
    }
}
