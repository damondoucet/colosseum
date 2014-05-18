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
            else
                OnPlatformCollision(true);

            CheckBounds();

            base.Update(gameTime);
        }

        public virtual void OnPlatformCollision(bool landedOnPlatform)
        { }

        private bool ShouldAddGravity(GameTime gameTime)
        {
            if (IgnoresGravity)
                return false;

            if (Velocity.Y > float.Epsilon)
            {
                var maybePlatformTile = TileOfPlatformFallingInto(gameTime);
                if (!maybePlatformTile.HasValue)
                    return true;

                TopLeftPosition.X += Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                TopLeftPosition.Y = maybePlatformTile.Value.Row * Stage.TileSize.Y - Height;
                Velocity.Y = 0;
                return false;
            }

            // Velocity.Y < -float.Epsilon -> going up -> we should fall
            return Velocity.Y < -float.Epsilon || !IsSittingOnPlatform();
        }

        private void AddGravity(GameTime gameTime)
        {
            Velocity.Y += Constants.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
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

            var afterVelocity = bottom + (float)gameTime.ElapsedGameTime.TotalSeconds * Velocity;
            var afterVelocityTile = Stage.GetRowColFromVector(afterVelocity);

            return afterVelocityTile.Row != bottomTile.Row &&
                    !CanFallThroughNextTile(afterVelocityTile) ?
                new RowCol?(afterVelocityTile) : null;
        }

        protected bool IsSittingOnPlatform()
        {
            var rowCol = Stage.GetRowColFromVector(
                TopLeftPosition + new Vector2(0, Constants.YPlatformCollisionAllowance));

            return !CanFallThroughNextTile(new RowCol(rowCol.Row + 1, rowCol.Col));
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

            if (TopLeftPosition.X <= 0 || TopLeftPosition.X >= Stage.Size.X - Width)
            {
                OnPlatformCollision(false);
                TopLeftPosition.X = Math.Max(0, Math.Min(TopLeftPosition.X, Stage.Size.X - Width));
            }

            if (TopLeftPosition.Y < 0)
            {
                OnPlatformCollision(false);
                TopLeftPosition.Y = 0;
                Velocity.Y = Math.Max(0, Velocity.Y);
            }

            // checking Y > StageHeight should be done by platforms
        }
    }
}
