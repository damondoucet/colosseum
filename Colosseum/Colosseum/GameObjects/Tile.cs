using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.GameObjects
{
    class Tile : GameObject
    {
        private readonly string _assetName;

        private readonly bool _canBeDroppedThrough;
        public bool CanBeDroppedThrough { get { return _canBeDroppedThrough; } }

        public virtual bool IsEmpty { get { return false; } }

        public Tile(Stage stage, Vector2 topLeftPosition, string assetName, bool canBeDroppedThrough)
            : base(stage, topLeftPosition)
        {
            _assetName = assetName;
            _canBeDroppedThrough = canBeDroppedThrough;
        }

        protected override List<Asset> ComputeAssets()
        {
            return new List<Asset>()
            {
                new Asset(Stage, _assetName, TopLeftPosition)
            };
        }
    }

    class EmptyTile : Tile
    {
        public override bool IsEmpty { get { return true; } }

        public EmptyTile(Stage stage)
            : base(stage, Vector2.Zero, "", true)
        {
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            // no-op
        }
    }
}
