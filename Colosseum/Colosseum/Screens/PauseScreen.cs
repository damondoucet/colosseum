using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Colosseum.Screens
{
    class PauseScreen : Screen
    {
        private readonly InputHelper _inputHelper;

        public PauseScreen(ScreenManager screenManager, InputHelper inputHelper)
            : base(screenManager)
        {
            _inputHelper = inputHelper;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isTopMost)
        {
        }
        
        public override void Update(GameTime gameTime)
        {
            if (_inputHelper.ShouldTogglePause(gameTime))
                ScreenManager.PopScreen();
        }
    }
}
