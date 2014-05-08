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

        protected Vector2 TopLeftPosition;

        protected Dictionary<string, Texture2D> AssetNameToTexture;

        public DrawableGameObject(Vector2 topLeftPosition, List<string> assetNames)
        {
            TopLeftPosition = topLeftPosition;
            AssetNames = assetNames;
        }

        // only called after AssetNameToTexture has already been loaded
        protected abstract Dictionary<string, Vector2> ComputeAssetNameToOffset();

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
                batch.Draw(AssetNameToTexture[assetName], TopLeftPosition + assetNameToOffset[assetName], Color.White);
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
                { AssetNames[0], Vector2.Zero }
            };
        }
    }
}
