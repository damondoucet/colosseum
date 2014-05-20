using Colosseum.GameObjects.Attacks.Melee;
using Colosseum.GameObjects.Collisions;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Attacks.Projectiles
{
    class NinjaBomb : Projectile
    {
        private readonly Ninja _ninja;

        protected override double PhaseInTime { get { return 0; } }
        protected override double TimeToLive { get { return Constants.Fighters.Ninja.Abilities.Bomb.DormantTime;  } }

        private static Vector2 AssetSize { get { return TextureDictionary.FindTextureSize(Constants.GameAssets.Ninja.Bomb); } }

        public override int Width { get { return (int)AssetSize.X; } }
        public override int Height { get { return (int)AssetSize.Y; } }

        public override bool IgnoresBounds { get { return false; } }
        public override bool IgnoresGravity { get { return false; } }
        public override bool IgnoresPlatforms { get { return false; } }

        public override Collideable ComputeCollideable()
        {
            return new NonCollideable();
        }

        protected override List<Asset> ComputeAssets()
        {
            return new Asset(Stage, Constants.GameAssets.Ninja.Bomb, TopLeftPosition).SingleToList();
        }

        public NinjaBomb(Ninja ninja)
            : base(ninja, ComputeSpawnPosition(ninja), Vector2.Zero)
        {
            _ninja = ninja;
        }

        private static Vector2 ComputeSpawnPosition(Ninja ninja)
        {
            return ninja.ComputeCenter() - AssetSize / 2;
        }

        public override void ExitStage()
        {
            _ninja.OnBombExploding();
            base.ExitStage();

            Stage.AddAttack(new Explosion(Source, TopLeftPosition + new Vector2(Width, Height) / 2));
        }
    }
}
