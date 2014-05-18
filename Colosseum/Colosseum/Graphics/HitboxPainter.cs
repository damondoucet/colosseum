using Colosseum.GameObjects.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.Graphics
{
    class HitboxPainter
    {
        public static void MaybePaintHitbox(SpriteBatch batch, Collideable collideable, Vector2 center)
        {
            MaybePaintHitbox(batch, collideable);

            // kinda sucks this has to be copied over but whatever
            var texture = TextureDictionary.Get(Constants.Assets.HitboxTestPoint);
            var size = new Vector2(texture.Width, texture.Height);

            DrawPoint(batch, texture, center, size);
        }

        public static void MaybePaintHitbox(SpriteBatch batch, Collideable collideable)
        {
            if (!Constants.DisplayHitboxTestPoints)
                return;

            var texture = TextureDictionary.Get(Constants.Assets.HitboxTestPoint);
            var size = new Vector2(texture.Width, texture.Height);

            foreach (var point in collideable.TestPoints)
                DrawPoint(batch, texture, point, size);
        }

        private static void DrawPoint(SpriteBatch batch, Texture2D texture, Vector2 point, Vector2 size)
        {
            var topLeft = point - size / 2;
            batch.Draw(
                texture,
                new Rectangle((int)topLeft.X, (int)topLeft.Y, texture.Width, texture.Height),
                Color.White);
        }
    }
}
