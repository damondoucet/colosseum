using Microsoft.Xna.Framework;

namespace Colosseum.GameObjects.Attacks
{
    abstract class TimedAttack : Attack
    {
        protected abstract double PhaseInTime { get; }
        protected abstract double TimeToLive { get; }

        protected double TimeAlive;

        protected abstract void OnPhaseInCompleted();

        public TimedAttack(Stage stage, Vector2 position, string assetName)
            : base(stage, position, assetName)
        {
            TimeAlive = 0;
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
