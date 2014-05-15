using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum
{
    // using a singleton here to cache textures
    // the alternative is that every class manages its own textures which gets unwieldy after a while
    static class TextureDictionary
    {
        private static ContentManager _contentManager;
        private static Dictionary<string, Texture2D> _textures;
        
        public static void SetContentManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _textures = new Dictionary<string, Texture2D>();
        }

        public static void Load(string assetName)
        {
            if (!_textures.ContainsKey(assetName))
                _textures[assetName] = _contentManager.Load<Texture2D>(assetName);
        }

        public static Texture2D Get(string assetName)
        {
            if (!_textures.ContainsKey(assetName))
                Load(assetName);

            return _textures[assetName];
        }

        public static Vector2 FindTextureSize(string assetName)
        {
            var texture = Get(assetName);
            return new Vector2(texture.Width, texture.Height);
        }
    }
}
