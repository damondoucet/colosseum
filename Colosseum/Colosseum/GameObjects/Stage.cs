using Colosseum.GameObjects.Attacks;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Microsoft.Xna.Framework;
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

        public int Winner { get; set; }  // -1 if no winner yet
        public bool IsPaused { get; set; }

        private readonly List<Attack> _attacks;
        private readonly List<Fighter> _fighters;

        public Stage()
            : base(null, Vector2.Zero)
        {
            this.Stage = this;  // bit of a hack, but whatever

            Winner = -1;

            _fighters = new List<Fighter>();
            _attacks = new List<Attack>();

            Size = new Vector2(Constants.Width, Constants.StageHeight);

            InitializeTestStage();
        }

        private void InitializeTestStage()
        {
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

            platforms.ForEach(p => p.UpdateTileMap(this));

            // doing it this way rather than starting with emptytiles prevents
            // an ArrayTypeMismatchException which is pretty silly
            for (int y = 0; y < yTiles; y++)
                for (int x = 0; x < xTiles; x++)
                    if (Tiles[y][x] == null)
                        Tiles[y][x] = new EmptyTile(this);
        }

        public void AddFighter(Fighter fighter)
        {
            _fighters.Add(fighter);
        }

        public RowCol GetRowColFromVector(Vector2 position)
        {
            int col = (int)(position.X / TileSize.X),
                row = (int)(position.Y / TileSize.Y);

            return new RowCol(row, col);
        }

        protected override List<Asset> ComputeAssets()
        {
            return new Asset(this, Constants.GameAssets.Background, Vector2.Zero)
                .SingleToList();
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);

            for (int y = 0; y < Tiles.Length; y++)
                for (int x = 0; x < Tiles[y].Length; x++)
                    Tiles[y][x].Draw(batch, gameTime);

            _fighters.ForEach(f => f.Draw(batch, gameTime));

            _attacks.ForEach(p => p.Draw(batch, gameTime));
        }

        public void AddAttack(Attack attack)
        {
            _attacks.Add(attack);
        }

        public void RemoveAttack(Attack attack)
        {
            _attacks.Remove(attack);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _fighters.ForEach(f => f.Update(gameTime));
            _attacks.ForEach(a => a.Update(gameTime));

            var attacks = new List<Attack>(_attacks);  // clone the list so it can be modified while we iterate on it

            for (int i = 0; i < attacks.Count; i++)
            {
                var attack = attacks[i];

                for (int j = 0; j < attacks.Count; j++)
                    if (i != j && attack.HasCollisionWithAttack(attacks[j]))
                        attack.OnAttackCollision(attacks[j]);

                foreach (var fighter in _fighters)
                    if (attack.HasCollisionWithFighter(fighter))
                        attack.OnFighterCollision(fighter);
            }
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

        public void UpdateTileMap(Stage stage)
        {
            for (int y = Top; y < Top + Height; y++)
                for (int x = Left; x < Left + Width; x++)
                    stage.Tiles[y][x] = new Tile(
                        stage,
                        new Vector2(x * TileSize.X, y * TileSize.Y), 
                        Constants.GameAssets.Platform, 
                        CanBeDroppedThrough);
        }
    }
}
