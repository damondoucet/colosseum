using Colosseum.GameObjects;
using Colosseum.Screens;
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

        private readonly ScreenManager _screenManager;

        public ColosseumGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Constants.Width;
            _graphics.PreferredBackBufferHeight = Constants.Height;
            _graphics.PreferMultiSampling = true;

            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;

            TextureDictionary.SetContentManager(Content);
            _screenManager = new ScreenManager();
            _screenManager.PushScreen(new FightScreen(_screenManager));
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // _gameComponents.ForEach(gc => gc.LoadContent(Content));
            // _gameOverTexture = Content.Load<Texture2D>("game_over");

            HitboxPainter.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _screenManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _screenManager.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
