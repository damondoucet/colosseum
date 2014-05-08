using Microsoft.Xna.Framework;
using System;

namespace Colosseum
{
    class Fighter : SimpleDrawableGameObject
    {
        private const int Width = 64;

        private readonly Stage _stage;

        public Vector2 Velocity { get; set; }

        public Fighter(Stage stage, Vector2 position)
            : base(position, Constants.Assets.FighterAsset)
        {
            _stage = stage;
            Velocity = Vector2.Zero;
        }

        private bool CanFall()
        {
            int x = (int)(TopLeftPosition.X / _stage.TileSize.X),
                y = (int)(TopLeftPosition.Y / _stage.TileSize.Y);

            return _stage.Tiles[y + 1][x].IsEmpty;
        }

        private float GetCurrentTileTop()
        {
            return _stage.TileSize.Y * (int)(TopLeftPosition.Y / _stage.TileSize.Y);
        }

        public override void Update(GameTime gameTime)
        {
            PerformGravity(gameTime);

            TopLeftPosition += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            CheckBounds();

            base.Update(gameTime);
        }

        private void PerformGravity(GameTime gameTime)
        {
            if (CanFall())
                Velocity += new Vector2(0, Constants.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            else
            {
                TopLeftPosition = new Vector2(TopLeftPosition.X, GetCurrentTileTop());
                Velocity = new Vector2(Velocity.X, 0);
            }
        }

        private void CheckBounds()
        {
            TopLeftPosition.X = Math.Max(0, Math.Min(TopLeftPosition.X, _stage.Size.X - Width));
            TopLeftPosition.Y = Math.Max(0, TopLeftPosition.Y);  // bottom of stage should be caught by platforms
        }
    }
}
