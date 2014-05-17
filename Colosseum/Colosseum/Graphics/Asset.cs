using Colosseum.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.Graphics
{
    class Asset
    {
        private readonly Stage _stage;

        public string Name { get; set; }
        public Vector2 TopLeftPosition { get; set; }
        public float Rotation { get; set; }
        public Color Tint { get; set; }
        public SpriteEffects SpriteEffects { get; set; }

        public Asset(Stage stage, string name, Vector2 topLeftPosition)
            : this(stage, name, topLeftPosition, 0.0f, Color.White, SpriteEffects.None)
        {
        }

        public Asset(Stage stage, string name, Vector2 topLeftPosition, float rotation)
            : this(stage, name, topLeftPosition, rotation, Color.White, SpriteEffects.None)
        { 
        }

        public Asset(Stage stage, string name, Vector2 topLeftPosition, float rotation, Color tint, SpriteEffects spriteEffects)
        {
            _stage = stage;

            Name = name;
            TopLeftPosition = topLeftPosition;
            Rotation = rotation;
            Tint = tint;
            SpriteEffects = spriteEffects;
        }

        private Color ComputeTint()
        {
            return _stage.GameOver
                ? Constants.GameOverTint
                : _stage.IsPaused
                    ? Constants.PausedTint
                    : Tint;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var texture = TextureDictionary.Get(Name);
            var position = TopLeftPosition + TextureDictionary.FindTextureSize(Name) / 2.0f;

            var rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            var tint = ComputeTint();

            spriteBatch.Draw(texture, rect, null, ComputeTint(), Rotation,
                new Vector2(texture.Width, texture.Height) / 2.0f, SpriteEffects, 0f);
        }
    }
}
