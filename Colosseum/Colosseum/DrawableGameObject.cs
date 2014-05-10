using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Colosseum
{
    abstract class DrawableGameObject
    {
        protected readonly List<string> AssetNames;

        protected Vector2 TopLeftPosition;  // this isn't a standard property because we want to be able to do Position.Y += ...

        protected Dictionary<string, Texture2D> AssetNameToTexture;

        public DrawableGameObject(Vector2 topLeftPosition, List<string> assetNames)
        {
            TopLeftPosition = topLeftPosition;
            AssetNames = assetNames;
        }

        // only called after AssetNameToTexture has already been loaded
        protected abstract Dictionary<string, Vector2> ComputeAssetNameToOffset();

        public virtual float GetAssetRotation(string assetName)
        {
            return 0.0f;
        }

        public virtual SpriteEffects GetAssetSpriteEffects(string assetName)
        {
            return SpriteEffects.None;
        }

        protected Vector2 FindTextureSize(string assetName)
        {
            var texture = AssetNameToTexture[assetName];
            return new Vector2(texture.Width, texture.Height);
        }

        public virtual void LoadContent(ContentManager content)
        {
            AssetNameToTexture = AssetNames.ToDictionary(x => x, x => content.Load<Texture2D>(x));
        }

        public virtual void Draw(SpriteBatch batch, GameTime gameTime)
        {
            var assetNameToOffset = ComputeAssetNameToOffset();

            foreach (var assetName in AssetNames)
            {
                var texture = AssetNameToTexture[assetName];
                var angle = GetAssetRotation(assetName);
                var position = TopLeftPosition + assetNameToOffset[assetName];
                var spriteEffects = GetAssetSpriteEffects(assetName);

                var rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

                batch.Draw(texture, rect, null, Color.White, angle, 
                    new Vector2(texture.Width, texture.Height) / 2.0f, spriteEffects, 0f);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }

    class SimpleDrawableGameObject : DrawableGameObject
    {
        public SimpleDrawableGameObject(Vector2 topLeftPosition, string assetName)
            : base(topLeftPosition, new List<string>() { assetName })
        {
        }

        protected override Dictionary<string, Vector2> ComputeAssetNameToOffset()
        {
            var size = FindTextureSize(AssetNames[0]);

            return new Dictionary<string, Vector2>
            {
                { AssetNames[0], size / 2.0f }
            };
        }
    }
}
