using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Colosseum
{
    public class ColosseumGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly List<DrawableGameObject> _gameComponents;
        private readonly Stage _stage;

        public ColosseumGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Constants.Width;
            _graphics.PreferredBackBufferHeight = Constants.Height;
            _graphics.PreferMultiSampling = true;

            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;

            _stage = new Stage();

            _gameComponents = new List<DrawableGameObject>() { _stage };
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameComponents.ForEach(gc => gc.LoadContent(Content));
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            _gameComponents.ForEach(gc => gc.Update(gameTime));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _gameComponents.ForEach(gc => gc.Draw(_spriteBatch, gameTime));
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
