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

        public Vector2 Size { get; set; }

        public Asset(Stage stage, string name, Vector2 topLeftPosition)
            : this(stage, name, topLeftPosition, 0.0f, Color.White, SpriteEffects.None)
        {
        }

        public Asset(Stage stage, string name, Vector2 topLeftPosition, float rotation)
            : this(stage, name, topLeftPosition, rotation, Color.White, SpriteEffects.None)
        { 
        }

        public Asset(Stage stage, string name, Vector2 topLeftPosition, float rotation, Color tint, SpriteEffects spriteEffects)
            : this(stage, name, topLeftPosition, TextureDictionary.FindTextureSize(name), rotation, tint, spriteEffects)
        { 
        }

        // WARNING: because XNA acts ridiculously, specifying size will produce undefined behavior 
        // unless the texture is a static color
        public Asset(Stage stage, string name, Vector2 topLeftPosition, Vector2 size, float rotation, Color tint, SpriteEffects spriteEffects)
        {
            _stage = stage;

            Name = name;
            TopLeftPosition = topLeftPosition;
            Size = size;
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
            var position = TopLeftPosition + Size / 2.0f;

            var destRect = new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y);
            var srcRect = new Rectangle(0, 0, destRect.Width, destRect.Height);

            var tint = ComputeTint();

            spriteBatch.Draw(texture, destRect, srcRect, ComputeTint(), Rotation,
                Size / 2.0f, SpriteEffects, 0f);
        }
    }
}
