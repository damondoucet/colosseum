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

        // this is a pretty poorly named method :/
        // returns null if not falling into any platform
        // assumes Velocity.Y > 0
        private RowCol? TileOfPlatformFallingInto(GameTime gameTime)
        {
            var bottom = new Vector2(TopLeftPosition.X, TopLeftPosition.Y + Height);
            var bottomTile = _stage.GetRowColFromVector(bottom);

            var afterVelocity = bottom + (float)gameTime.ElapsedGameTime.TotalSeconds * Velocity;
            var afterVelocityTile = _stage.GetRowColFromVector(afterVelocity);

            return afterVelocityTile.Row != bottomTile.Row &&
                    !CanFallThroughNextTile(afterVelocityTile) ?
                new RowCol?(afterVelocityTile) : null;
        }

        private bool CanFallThroughNextTile(RowCol rowCol)
        {
            return rowCol.Row < _stage.Tiles.Length &&
                _stage.Tiles[rowCol.Row][rowCol.Col].IsEmpty;
        }
        
        private bool IsSittingOnPlatform()
        {
            var rowCol = _stage.GetRowColFromVector(
                TopLeftPosition + new Vector2(0, Constants.YPlatformCollisionAllowance));

            return !CanFallThroughNextTile(new RowCol(rowCol.Row + 1, rowCol.Col));
        }

        private float GetCurrentTileTop()
        {
            return _stage.TileSize.Y * (_stage.GetRowColFromVector(TopLeftPosition).Row + 1) - Height;
        }

        public override void Update(GameTime gameTime)
        {
            var shouldAddGravity = ShouldAddGravity(gameTime);

            TopLeftPosition += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (shouldAddGravity)
                AddGravity(gameTime);

            CheckBounds();

            base.Update(gameTime);
        }

        private void AddGravity(GameTime gameTime)
        {
            Velocity.Y += Constants.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private bool ShouldAddGravity(GameTime gameTime)
        {
            if (Velocity.Y > float.Epsilon)
            {
                var maybePlatformTile = TileOfPlatformFallingInto(gameTime);
                if (!maybePlatformTile.HasValue)
                    return true;

                TopLeftPosition.X += Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                TopLeftPosition.Y = maybePlatformTile.Value.Row * _stage.TileSize.Y - Height;
                Velocity.Y = 0;
                return false;
            }

            // Velocity.Y < -float.Epsilon -> going up -> we should fall
            return Velocity.Y < -float.Epsilon || !IsSittingOnPlatform();
        }

        private void CheckBounds()
        {
            TopLeftPosition.X = Math.Max(0, Math.Min(TopLeftPosition.X, _stage.Size.X - Width));

            if (TopLeftPosition.Y < 0) 
            {
                TopLeftPosition.Y = 0;
                Velocity.Y = Math.Max(0, Velocity.Y);
            }

            // checking Y > StageHeight should be done by platforms
        }

        public void HandleAction(InputHelper.Action action, Vector2 rightThumbstick)
        {
            switch (action)
            {
                case InputHelper.Action.Jump:
                    if (IsSittingOnPlatform() && Velocity.Y == 0)
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
