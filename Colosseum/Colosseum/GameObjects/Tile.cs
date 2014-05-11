using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Colosseum.GameObjects
{
    class Tile : GameObject
    {
        private readonly bool _canBeDroppedThrough;
        public bool CanBeDroppedThrough { get { return _canBeDroppedThrough; } }

        public virtual bool IsEmpty { get { return false; } }

        public Tile(Vector2 topLeftPosition, string assetName, bool canBeDroppedThrough)
            : base(topLeftPosition, assetName)
        {
            _canBeDroppedThrough = canBeDroppedThrough;
        }
    }

    class EmptyTile : Tile
    {
        public override bool IsEmpty { get { return true; } }

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
