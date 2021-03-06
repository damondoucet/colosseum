using Colosseum.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Colosseum.Screens
{
    class PauseScreen : Screen
    {
        public override bool IsModal { get { return true; } }

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
            if (_inputHelper.PauseToggled())
                ScreenManager.PopScreen();
        }
    }
}
