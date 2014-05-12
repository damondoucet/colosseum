using Colosseum.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Colosseum
{
    public class ColosseumGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly List<GameObject> _gameComponents;
        private readonly Stage _stage;
        private readonly Fighter[] _fighters;

        private Texture2D _gameOverTexture;

        private readonly InputHelper _inputHelper;

        public ColosseumGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Constants.Width;
            _graphics.PreferredBackBufferHeight = Constants.Height;
            _graphics.PreferMultiSampling = true;

            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;

            _stage = new Stage();
            _fighters = new[]
            {
                new Fighter(_stage, new Vector2(350f, 300f), 0),
                new Fighter(_stage, new Vector2(950f, 300f), -MathHelper.Pi),
            };

            foreach (var fighter in _fighters)
                _stage.AddFighter(fighter);

            _gameComponents = new List<GameObject>() { _stage };
            _gameComponents.AddRange(_fighters);

            _inputHelper = new InputHelper(_fighters);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _gameComponents.ForEach(gc => gc.LoadContent(Content));
            _gameOverTexture = Content.Load<Texture2D>("game_over");


            HitboxPainter.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (!_stage.GameOver)
            {
                _inputHelper.CheckInput();

                _gameComponents.ForEach(gc => gc.Update(gameTime));
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _gameComponents.ForEach(gc => gc.Draw(_spriteBatch, gameTime));

            if (_stage.GameOver)
                _spriteBatch.Draw(_gameOverTexture, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.White);
                
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
