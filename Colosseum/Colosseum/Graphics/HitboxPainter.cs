using Colosseum.GameObjects.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.Graphics
{
    class HitboxPainter
    {
        public static void MaybePaintHitbox(SpriteBatch batch, Collideable collideable)
        {
            if (!Constants.DisplayHitboxTestPoints)
                return;

            var texture = TextureDictionary.Get(Constants.Assets.HitboxTestPoint);

            var size = new Vector2(texture.Width, texture.Height);

            foreach (var point in collideable.TestPoints)
            {
                var topLeft = point - size / 2;
                batch.Draw(
                    texture,
                    new Rectangle((int)topLeft.X, (int)topLeft.Y, texture.Width, texture.Height),
                    Color.White);
            }
        }
    }
}
