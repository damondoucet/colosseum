using Colosseum.GameObjects.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.Graphics
{
    class HitboxPainter
    {
        public static void MaybePaintHitbox(SpriteBatch batch, Collideable collideable, Vector2 center)
        {
            if (!Constants.DisplayHitboxTestPoints)
                return;

            MaybePaintHitbox(batch, collideable);
            
            DrawPoint(batch, center);
        }

        public static void MaybePaintHitbox(SpriteBatch batch, Collideable collideable)
        {
            if (!Constants.DisplayHitboxTestPoints)
                return;

            foreach (var point in collideable.TestPoints)
                DrawPoint(batch, point);
        }

        private static void DrawPoint(SpriteBatch batch, Vector2 point)
        {
            var texture = TextureDictionary.Get(Constants.GameAssets.HitboxTestPoint);
            var size = new Vector2(texture.Width, texture.Height);

            var topLeft = point - size / 2;

            batch.Draw(
                texture,
                new Rectangle((int)topLeft.X, (int)topLeft.Y, texture.Width, texture.Height),
                Color.White);
        }
    }
}
