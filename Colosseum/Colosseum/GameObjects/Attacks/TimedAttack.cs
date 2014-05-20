using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;

namespace Colosseum.GameObjects.Attacks
{
    abstract class TimedAttack : Attack
    {
        protected abstract double PhaseInTime { get; }
        protected abstract double TimeToLive { get; }

        protected double TimeAlive;

        public TimedAttack(Fighter source, Vector2 position)
            : base(source, position)
        {
            TimeAlive = 0;
        }

        protected override bool ShouldExit()
        {
            return TimeAlive > TimeToLive + PhaseInTime ||
                base.ShouldExit();
        }

        protected virtual void OnPhaseInCompleted()
        {
        }

        public override void Update(GameTime gameTime)
        {
            var dt = gameTime.ElapsedGameTime.TotalSeconds;

            TimeAlive += dt;

            if (TimeAlive - dt < PhaseInTime && TimeAlive >= PhaseInTime)
                OnPhaseInCompleted();

            base.Update(gameTime);
        }
    }
}
