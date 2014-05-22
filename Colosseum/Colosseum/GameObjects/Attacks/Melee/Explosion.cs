using Colosseum.GameObjects.Attacks.Projectiles;
using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Melee
{
    class Explosion : TimedMeleeAttack
    {
        private Vector2 _center;

        public override bool CollisionIgnoresSource { get { return false; } }

        protected override double PhaseInTime { get { return 0; } }
        protected override double TimeToLive { get { return Constants.Fighters.Explosion.Length; } }

        private double _size;

        public override int Width { get { return (int)_size; } }
        public override int Height { get { return (int)_size; } }

        public Explosion(Fighter source, Vector2 center)
            : base(source)
        {
            _center = center;
            _size = 0;
        }

        public override bool HasCollisionWithAttack(Attack attack)
        {
            return base.HasCollisionWithAttack(attack);
        }

        public override bool HasCollisionWithFighter(Fighter fighter)
        {
            return base.HasCollisionWithFighter(fighter);
        }

        public override void OnFighterCollision(Fighter fighter)
        {
            AddKnockbackToFighter(fighter);

            base.OnFighterCollision(fighter);
        }

        private void AddKnockbackToFighter(Fighter fighter)
        {
            var fighterCenter = fighter.ComputeCenter();

            // if we're at the center of the blast, get blasted upwards
            var delta = fighterCenter == _center ? new Vector2(0, -1) : fighterCenter - _center;
            
            var force = delta.Norm() * Constants.Fighters.Explosion.KnockbackForce;
            System.Console.WriteLine(force);
            var kb = new KnockbackForce(Source, fighter, Constants.Fighters.Explosion.KnockbackTime, force);
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
            _size += gameTime.ElapsedGameTime.TotalSeconds * Constants.Fighters.Explosion.Scale;

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
