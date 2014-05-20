using Colosseum.GameObjects.Attacks.Projectiles;
using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Melee
{
    class NinjaBombExplosion : TimedMeleeAttack
    {
        private Vector2 _center;

        protected override double PhaseInTime { get { return 0; } }
        protected override double TimeToLive { get { return Constants.Fighters.Ninja.Abilities.Bomb.ExplosionLength; } }

        private double _size;

        public override int Width { get { return (int)_size; } }
        public override int Height { get { return (int)_size; } }

        public NinjaBombExplosion(Fighter source, Vector2 center)
            : base(source)
        {
            _center = center;
            _size = 0;
        }

        public override void OnFighterCollision(Fighter fighter)
        {
            AddKnockbackToFighter(fighter);

            base.OnFighterCollision(fighter);
        }

        private void AddKnockbackToFighter(Fighter fighter)
        {
            var vector = (fighter.ComputeCenter() - _center).Norm();
            var force = vector * Constants.Fighters.Ninja.Abilities.Bomb.KnockbackForce;
            var kb = new KnockbackForce(Source, fighter, Constants.Fighters.Ninja.Abilities.Bomb.KnockbackTime, force);
            Stage.AddAttack(kb);
        }

        public override void ExitStage()
        {
            base.ExitStage();
        }

        public override Vector2 ComputeCenter()
        {
            return _center;
        }

        public override Collideable ComputeCollideable()
        {
            return new Rect(_center, Width, Height, 0);
        }

        public override void Update(GameTime gameTime)
        {
            _size += gameTime.ElapsedGameTime.TotalSeconds * Constants.Fighters.Ninja.Abilities.Bomb.ExplosionScale;

            TopLeftPosition = _center - new Vector2(Width, Height) / 2;

            base.Update(gameTime);
        }

        protected override List<Asset> ComputeAssets()
        {
            var size = new Vector2(Width, Height);

            return new Asset(
                Stage, 
                Constants.GameAssets.Ninja.BombExplosion, 
                _center - size / 2, 
                size, 
                0, 
                Color.White, 
                SpriteEffects.None
            ).SingleToList();
        }
    }
}
