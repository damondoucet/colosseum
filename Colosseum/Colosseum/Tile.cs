using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Colosseum
{
    class Tile : SimpleDrawableGameObject
    {
        private readonly bool _canBeDroppedThrough;
        public bool CanBeDroppedThrough { get { return _canBeDroppedThrough; } }

        public Tile(Vector2 topLeftPosition, string assetName, bool canBeDroppedThrough)
            : base(topLeftPosition, assetName)
        {
            _canBeDroppedThrough = canBeDroppedThrough;
        }
    }

    class EmptyTile : Tile
    {
        public EmptyTile()
            : base(Vector2.Zero, "", true)
        {
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            // no-op
        }

        public override void LoadContent(ContentManager content)
        {
            // no-op
        }
    }
}
