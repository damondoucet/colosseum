using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;

namespace Colosseum.GameObjects.Attacks.Melee
{
    class Thrust : TimedMeleeAttack
    {
        private readonly Thruster _thruster;
        private readonly double _phaseInTime;
        private readonly double _timeToLive;

        protected override double PhaseInTime { get { return _phaseInTime; } }
        protected override double TimeToLive { get { return _timeToLive; } }

        public override int Width { get { return 0; } }
        public override int Height { get { return 0; } }

        public Thrust(Fighter source, Thruster thruster, double phaseInTime, double timeToLive)
            : base(source)
        {
            _thruster = thruster;
            _phaseInTime = phaseInTime;
            _timeToLive = timeToLive;
        }

        public override void ExitStage()
        {
            _thruster.OnThrustFinished();
            base.ExitStage();
        }

        public override Collideable ComputeCollideable()
        {
            return Source.ComputeWeaponCollideable();
        }
    }

    class ThrustFactory
    {
        public static Thrust CreateNinjaThrust(Ninja source)
        {
            return new Thrust(
                source,
                source,
                Constants.Fighters.Ninja.Abilities.Thrust.PhaseInTime,
                Constants.Fighters.Ninja.Abilities.Thrust.TimeToLive);
        }

        public static Thrust CreateKnightThrust(Knight source)
        {
            return new Thrust(
                source, 
                source, 
                Constants.Fighters.Knight.Abilities.Thrust.PhaseInTime, 
                Constants.Fighters.Knight.Abilities.Thrust.TimeToLive);
        }
    }
}
