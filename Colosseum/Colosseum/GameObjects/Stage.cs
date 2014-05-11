using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum.GameObjects
{
    struct RowCol
    {
        public int Row;
        public int Col;
        public RowCol(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    class Stage : GameObject
    {
        public Vector2 Size { get; set; }
        public Vector2 TileSize { get; set; }
        public Tile[][] Tiles { get; set; }

        public Stage()
            : base(Vector2.Zero, Constants.Assets.BackgroundAsset)
        {
            Size = new Vector2(1280, 720);

            var xTiles = 20;
            var yTiles = 12;

            Tiles = Enumerable.Range(0, yTiles)
                .Select(y => 
                    Enumerable.Repeat<Tile>(null, xTiles)
                        .ToArray())
                .ToArray();

            TileSize = new Vector2(64.0f, 64.0f);

            var platforms = new List<Platform>()
            {
                new Platform(TileSize, 5, 8, 6, 1, true),
                new Platform(TileSize, 8, 4, 4, 1, true),
                new Platform(TileSize, 8, 12, 4, 1, true),
                new Platform(TileSize, 11, 0, 20, 1, false)
            };

            platforms.ForEach(p => p.UpdateTileMap(Tiles));

            // doing it this way rather than starting with emptytiles prevents
            // an ArrayTypeMismatchException which is pretty silly
            for (int y = 0; y < yTiles; y++)
                for (int x = 0; x < xTiles; x++)
                    if (Tiles[y][x] == null)
                        Tiles[y][x] = new EmptyTile();
        }

        public RowCol GetRowColFromVector(Vector2 position)
        {
            int col = (int)(position.X / TileSize.X),
                row = (int)(position.Y / TileSize.Y);

            return new RowCol(row, col);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            for (int y = 0; y < Tiles.Length; y++)
                for (int x = 0; x < Tiles[y].Length; x++)
                    Tiles[y][x].LoadContent(content);
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);

            for (int y = 0; y < Tiles.Length; y++)
                for (int x = 0; x < Tiles[y].Length; x++)
                    Tiles[y][x].Draw(batch, gameTime);
        }
    }

    class Platform
    {
        // all values below are by tile
        public Vector2 TileSize { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool CanBeDroppedThrough { get; set; }

        public Platform(Vector2 tileSize, int top, int left, int width, int height, bool canBeDroppedThrough)
        {
            TileSize = tileSize;
            Top = top;
            Left = left;
            Width = width;
            Height = height;
            CanBeDroppedThrough = canBeDroppedThrough;
        }

        public void UpdateTileMap(Tile[][] tiles)
        {
            for (int y = Top; y < Top + Height; y++)
                for (int x = Left; x < Left + Width; x++)
                    tiles[y][x] = new Tile(
                        new Vector2(x * TileSize.X, y * TileSize.Y), 
                        Constants.Assets.PlatformAsset, 
                        CanBeDroppedThrough);
        }
    }
}
