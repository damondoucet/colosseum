using Colosseum.GameObjects;
using Colosseum.GameObjects.Fighters;
using Colosseum.Graphics;
using Colosseum.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.Screens
{
    class FightScreen : Screen
    {
        public override bool IsModal { get { return false; } }

        private readonly List<GameObject> _gameComponents;
        private readonly Stage _stage;

        private readonly InputHelper _inputHelper;
        private readonly FighterInputDispatcher _dispatcher;

        private double _gameOverTime;

        public FightScreen(ScreenManager screenManager, InputHelper inputHelper, Stage stage, Fighter[] fighters)
            : base(screenManager)
        {
            _stage = stage;

            foreach (var fighter in fighters)
                _stage.AddFighter(fighter);

            _inputHelper = inputHelper;
            _dispatcher = new FighterInputDispatcher(inputHelper, fighters);

            _gameOverTime = 0;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isTopMost)
        {
            _stage.IsPaused = !isTopMost;
            _stage.Draw(spriteBatch, gameTime);

            if (_stage.Winner != -1)
                spriteBatch.Draw(
                    TextureDictionary.Get(Constants.GameAssets.WinAssets[_stage.Winner]), 
                    new Rectangle(0, 0, Constants.Width, Constants.Height), 
                    Color.White);      
        }

        public override void Update(GameTime gameTime)
        {
            if (_stage.Winner != -1)
            {
                _gameOverTime += gameTime.ElapsedGameTime.TotalSeconds;

                if (_gameOverTime > Constants.GameOverTimeBeforeTransition)
                    ScreenManager.PopScreen();
                return;
            }

            if (_inputHelper.PauseToggled())
                ScreenManager.PushScreen(new PauseScreen(ScreenManager, _inputHelper));
            else
            {
                _dispatcher.CheckInput();
                _stage.Update(gameTime);
            }
        }
    }
}
