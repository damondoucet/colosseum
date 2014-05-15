using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.Screens
{
    class ScreenManager
    {
        private readonly List<Screen> _screens;

        public ScreenManager()
        {
            _screens = new List<Screen>();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < _screens.Count - 1; i++)
                _screens[i].Draw(spriteBatch, gameTime, false);

            _screens[_screens.Count - 1].Draw(spriteBatch, gameTime, true);
        }

        public void Update(GameTime gameTime)
        {
            _screens[_screens.Count - 1].Update(gameTime);
        }

        public void PushScreen(Screen screen)
        {
            _screens.Add(screen);
        }

        public void PopScreen()
        {
            _screens.RemoveAt(_screens.Count - 1);
        }
    }
}
