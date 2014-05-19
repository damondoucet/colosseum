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
        private readonly List<GameObject> _gameComponents;
        private readonly Stage _stage;
        private readonly Fighter[] _fighters;

        private readonly InputHelper _inputHelper;
        private readonly FighterInputDispatcher _dispatcher;

        public FightScreen(ScreenManager screenManager, InputHelper inputHelper)
            : base(screenManager)
        {
            _stage = new Stage();

            var fighterFactory = new FighterFactory(_stage);

            _fighters = new[]
            {
                fighterFactory.CreateFighter(FighterType.Ninja, 1),
                fighterFactory.CreateFighter(FighterType.Knight, 2)
            };

            foreach (var fighter in _fighters)
                _stage.AddFighter(fighter);

            _gameComponents = new List<GameObject>() { _stage };
            _gameComponents.AddRange(_fighters);

            _inputHelper = inputHelper;
            _dispatcher = new FighterInputDispatcher(inputHelper, _fighters);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isTopMost)
        {
            _stage.IsPaused = !isTopMost;
            _gameComponents.ForEach(gc => gc.Draw(spriteBatch, gameTime));

            if (_stage.GameOver)
                spriteBatch.Draw(
                    TextureDictionary.Get(Constants.Assets.GameOver), 
                    new Rectangle(0, 0, Constants.Width, Constants.Height), 
                    Color.White);      
        }

        public override void Update(GameTime gameTime)
        {
            if (_stage.GameOver)
                return;

            if (_inputHelper.PauseToggled())
                ScreenManager.PushScreen(new PauseScreen(ScreenManager, _inputHelper));
            else
            {
                _dispatcher.CheckInput();

                _gameComponents.ForEach(gc => gc.Update(gameTime));
            }
        }
    }
}
