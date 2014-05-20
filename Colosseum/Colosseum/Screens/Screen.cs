using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.Screens
{
    abstract class Screen
    {
        protected readonly ScreenManager ScreenManager;

        /// <summary>
        /// Whether screens can be drawn under this one or not
        /// </summary>
        public abstract bool IsModal { get; }

        public Screen(ScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isTopMost);
        public abstract void Update(GameTime gameTime);
    }
}
