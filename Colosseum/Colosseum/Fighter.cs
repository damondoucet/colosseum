using Microsoft.Xna.Framework;
using System;

namespace Colosseum
{
    class Fighter : SimpleDrawableGameObject
    {
        private const int Width = 64;
        private const int Height = 64;

        private readonly Stage _stage;

        public Vector2 Velocity;

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
                Velocity.Y += Constants.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                TopLeftPosition.Y = GetCurrentTileTop();
                Velocity.Y = Math.Min(0, Velocity.Y);
            }
        }

        private void CheckBounds()
        {
            TopLeftPosition.X = Math.Max(0, Math.Min(TopLeftPosition.X, _stage.Size.X - Width));

            if (TopLeftPosition.Y < 0)
            {
                TopLeftPosition.Y = 0;
                Velocity.Y = Math.Max(0, Velocity.Y);
            }
        }

        public void HandleAction(InputHelper.Action action, Vector2 rightThumbstick)
        {
            switch (action)
            {
                case InputHelper.Action.Jump:
                    if (!CanFall())
                        Velocity += new Vector2(0, -Constants.FighterJumpVelocity);
                    break;
                case InputHelper.Action.Left:
                    TopLeftPosition += new Vector2(-Constants.FighterMovementX, 0);
                    break;
                case InputHelper.Action.Right:
                    TopLeftPosition += new Vector2(Constants.FighterMovementX, 0);
                    break;
                default:
                    Console.WriteLine("Invalid action: {0}. Ignoring", action);
                    return;
            }
        }

        public void OnLeftThumbstick(Vector2 value)
        {
            var x = value.X;
            var y = value.Y;

            // TODO(ddoucet): walk slower depending on value of vector
            // TODO(ddoucet): this is getting sloppy. might want to refactor soon
            if (x < 0)
                HandleAction(InputHelper.Action.Left, Vector2.Zero);
            else if (x > 0)
                HandleAction(InputHelper.Action.Right, Vector2.Zero);

            if (y > 0)
                HandleAction(InputHelper.Action.Jump, Vector2.Zero);
        }

        public void OnRightThumbstick(Vector2 value)
        {
        }
    }
}
