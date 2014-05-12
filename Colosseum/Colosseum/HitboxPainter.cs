using Colosseum.GameObjects.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum
{
    class HitboxPainter
    {
        private static Texture2D Texture;

        public static void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>(Constants.Assets.HitboxTestPoint);
        }

        public static void MaybePaintHitbox(SpriteBatch batch, Collideable collideable)
        {
            if (!Constants.DisplayHitboxTestPoints)
                return;

            var size = new Vector2(Texture.Width, Texture.Height);

            foreach (var point in collideable.TestPoints)
            {
                var topLeft = point - size / 2;
                batch.Draw(Texture, new Rectangle((int)topLeft.X, (int)topLeft.Y, Texture.Width, Texture.Height), Color.White);
            }
        }
    }
}
