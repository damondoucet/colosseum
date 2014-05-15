using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.Screens
{
    abstract class Screen
    {
        protected readonly ScreenManager ScreenManager;

        public Screen(ScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isTopMost);
        public abstract void Update(GameTime gameTime);
    }
}
