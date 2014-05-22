using Microsoft.Xna.Framework;
using System;

namespace Colosseum.GameObjects
{
    abstract class MoveableGameObject : GameObject
    {
        public abstract int Width { get; }
        public abstract int Height { get; }

        public abstract bool IgnoresPlatforms { get; }
        public abstract bool IgnoresBounds { get; }
        public abstract bool IgnoresGravity { get; }

        public Vector2 Velocity;  // this isn't a standard property because we want to be able to do Velocity.Y += ...

        public MoveableGameObject(Stage stage, Vector2 topLeftPosition)
            : base(stage, topLeftPosition)
        {
        }

        public virtual Vector2 ComputeCenter()
        {
            return TopLeftPosition + new Vector2(Width, Height) / 2;
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

        // contact vector is e.g. (-1, 0) if hitting on left side
        public virtual void OnPlatformCollision(Vector2 contactVector)
        { }

        protected bool IsInPlatform()
        {
            var topLeftTile = Stage.GetRowColFromVector(TopLeftPosition);
            var botRightTile = Stage.GetRowColFromVector(TopLeftPosition + new Vector2(Width, Height));

            for (int row = Math.Max(0, topLeftTile.Row); row <= botRightTile.Row && row < Stage.Tiles.Length; row++)
                for (int col = Math.Max(0, topLeftTile.Col); col <= botRightTile.Col && col < Stage.Tiles[0].Length; col++)
                    if (!Stage.Tiles[row][col].IsEmpty)
                        return true;

            return false;
        }

        private bool ShouldAddGravity(GameTime gameTime)
        {
            if (Velocity.Y > float.Epsilon && !IgnoresPlatforms)
            {
                var maybePlatformTile = TileOfPlatformFallingInto(gameTime);
                if (!maybePlatformTile.HasValue)
                    return true;

                TopLeftPosition.X += Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                TopLeftPosition.Y = maybePlatformTile.Value.Row * Stage.TileSize.Y - Height;
                Velocity.Y = 0;
                OnPlatformCollision(new Vector2(0, 1));
                return false;
            }

            if (IgnoresGravity)
                return false;

            // Velocity.Y < -float.Epsilon -> going up -> we should fall
            return Velocity.Y < -float.Epsilon || !IsSittingOnPlatform();
        }

        private void AddGravity(GameTime gameTime)
        {
            Velocity.Y += Constants.Fighters.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // this is a pretty poorly named method :/
        // returns null if not falling into any platform
        // assumes Velocity.Y > 0
        private RowCol? TileOfPlatformFallingInto(GameTime gameTime)
        {
            if (IgnoresPlatforms)
                return null;

            var bottom = new Vector2(TopLeftPosition.X, TopLeftPosition.Y + Height);
            var bottomTile = Stage.GetRowColFromVector(bottom);

            var afterVelocityLeft = bottom + (float)gameTime.ElapsedGameTime.TotalSeconds * Velocity;
            var afterVelocityLeftTile = Stage.GetRowColFromVector(afterVelocityLeft);

            var afterVelocityRight = bottom + (float)gameTime.ElapsedGameTime.TotalSeconds * Velocity + new Vector2(Width, 0);
            var afterVelocityRightTile = Stage.GetRowColFromVector(afterVelocityRight);

            if (afterVelocityLeftTile.Row == bottomTile.Row)
                return null;

            for (int col = afterVelocityLeftTile.Col; col <= afterVelocityRightTile.Col && col < Stage.Tiles[0].Length; col++)
            { 
                var rc = new RowCol(afterVelocityLeftTile.Row, col);
                if (!CanFallThroughNextTile(rc))
                    return new RowCol?(rc);
            }

            return null;
        }

        public bool IsSittingOnPlatform()
        {
            var bottomLeftRowCol = Stage.GetRowColFromVector(
                TopLeftPosition + new Vector2(0, Constants.YPlatformCollisionAllowance + Height));

            var bottomRightRowCol = Stage.GetRowColFromVector(
                TopLeftPosition + new Vector2(Width, Constants.YPlatformCollisionAllowance + Height));

            for (int col = Math.Max(0, bottomLeftRowCol.Col); col <= bottomRightRowCol.Col && col < Stage.Tiles[0].Length; col++)
                if (!CanFallThroughNextTile(new RowCol(bottomLeftRowCol.Row, col)))
                    return true;

            return false;
        }

        private bool CanFallThroughNextTile(RowCol rowCol)
        {
            return rowCol.Row < Stage.Tiles.Length &&
                Stage.Tiles[rowCol.Row][rowCol.Col].IsEmpty;
        }

        private void CheckBounds()
        {
            if (IgnoresBounds)
                return;

            if (TopLeftPosition.X <= 0)
            {
                OnPlatformCollision(new Vector2(-1, 0));
                TopLeftPosition.X = 0;
            }
            if (TopLeftPosition.X >= Stage.Size.X - Width)
            {
                OnPlatformCollision(new Vector2(1, 0));
                TopLeftPosition.X = Stage.Size.X - Width;
            }

            if (TopLeftPosition.Y < 0)
            {
                OnPlatformCollision(new Vector2(0, -1));
                TopLeftPosition.Y = 0;
                Velocity.Y = Math.Max(0, Velocity.Y);
            }
            if (TopLeftPosition.Y >= Stage.Size.Y - Height)
            {
                OnPlatformCollision(new Vector2(0, 1));
                TopLeftPosition.Y = Stage.Size.Y - Height;
                Velocity.Y = 0;
            }

            // checking Y > StageHeight should be done by platforms
        }
    }
}
