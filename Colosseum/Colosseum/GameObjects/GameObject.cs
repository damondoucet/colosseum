using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum.GameObjects
{
    abstract class GameObject
    {
        protected Stage Stage;

        protected readonly List<string> AssetNames;

        protected Vector2 TopLeftPosition;  // this isn't a standard property because we want to be able to do Position.Y += ...

        public GameObject(Stage stage, Vector2 topLeftPosition, string assetName)
            : this(stage, topLeftPosition, new List<string> { assetName })
        { }

        public GameObject(Stage stage, Vector2 topLeftPosition, List<string> assetNames)
        {
            Stage = stage;
            TopLeftPosition = topLeftPosition;
            AssetNames = assetNames;
        }

        public GameObject(Stage stage, Vector2 topLeftPosition, Dictionary<string, Texture2D> assetNameToTexture)
        {
            Stage = stage;
            TopLeftPosition = topLeftPosition;
            AssetNames = assetNameToTexture.Select(kvp => kvp.Key).ToList();
        }

        // only called after AssetNameToTexture has already been loaded
        protected virtual Dictionary<string, Vector2> ComputeAssetNameToOffset()
        {
            return AssetNames.ToDictionary(
                assetName => assetName,
                assetName => TextureDictionary.FindTextureSize(assetName) / 2.0f);
        }

        public virtual float GetAssetRotation(string assetName)
        {
            return 0.0f;
        }

        public virtual SpriteEffects GetAssetSpriteEffects(string assetName)
        {
            return SpriteEffects.None;
        }

        public virtual Color GetAssetTint(string assetName)
        {
            return Color.White;
        }

        private Color GetTint(string assetName)
        {
            return Stage.GameOver ? Constants.GameOverTint
                : Stage.IsPaused ? Constants.PausedTint
                : GetAssetTint(assetName);
        }

        public virtual void Draw(SpriteBatch batch, GameTime gameTime)
        {
            var assetNameToOffset = ComputeAssetNameToOffset();

            foreach (var assetName in AssetNames)
            {
                var texture = TextureDictionary.Get(assetName);
                var angle = GetAssetRotation(assetName);
                var position = TopLeftPosition + assetNameToOffset[assetName];
                var tint = GetTint(assetName);
                var spriteEffects = GetAssetSpriteEffects(assetName);

                var rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

                batch.Draw(texture, rect, null, tint, angle, 
                    new Vector2(texture.Width, texture.Height) / 2.0f, spriteEffects, 0f);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
