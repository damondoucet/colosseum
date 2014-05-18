using Colosseum.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.Screens
{
    class ScreenManager
    {
        private readonly InputHelper _inputHelper;
        private readonly List<Screen> _screens;

        public ScreenManager()
        {
            _inputHelper = new InputHelper();

            _screens = new List<Screen>();
            _screens.Add(new FightScreen(this, _inputHelper));
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < _screens.Count - 1; i++)
                _screens[i].Draw(spriteBatch, gameTime, false);

            _screens[_screens.Count - 1].Draw(spriteBatch, gameTime, true);
        }

        public void Update(GameTime gameTime)
        {
            _inputHelper.UpdateStates();
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
