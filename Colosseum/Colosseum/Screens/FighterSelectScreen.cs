using Colosseum.Graphics;
using Colosseum.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Colosseum.Screens
{
    class FighterSelectScreen : Screen
    {
        class SelectScreenAsset
        {
            public string Name;
            public int X;
            public int Y;

            public SelectScreenAsset(string name, int x, int y)
            {
                Name = name;
                X = x;
                Y = y;
            }

            private Rectangle ComputeRectangle()
            { 
                var size = TextureDictionary.FindTextureSize(Name);
                return new Rectangle(X, Y, (int)size.X, (int)size.Y);
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(TextureDictionary.Get(Name), ComputeRectangle(), Color.White);
            }
        }

        private static string[] PlayerSelectAssets = new[] { Constants.FighterSelect.PlayerOneSelectAsset, Constants.FighterSelect.PlayerTwoSelectAsset };
        
        private int[] _playerFighterIndices;

        public override bool IsModal { get { return false; } }

        private readonly InputHelper _inputHelper;

        public FighterSelectScreen(ScreenManager screenManager, InputHelper inputHelper)
            : base(screenManager)
        {
            _playerFighterIndices = new[] { 0, 0 };
            _inputHelper = inputHelper;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isTopMost)
        {
            foreach (var asset in ComputeAssets())
                asset.Draw(spriteBatch);
        }

        private List<SelectScreenAsset> ComputeAssets()
        {
            return new List<SelectScreenAsset>()
            {
                new SelectScreenAsset(Constants.FighterSelect.LogoTextAsset, Constants.FighterSelect.LogoX, Constants.FighterSelect.LogoY),
                new SelectScreenAsset(Constants.FighterSelect.KnightTextAsset, Constants.FighterSelect.NameX, Constants.FighterSelect.KnightY),
                new SelectScreenAsset(Constants.FighterSelect.NinjaTextAsset, Constants.FighterSelect.NameX, Constants.FighterSelect.NinjaY),
                new SelectScreenAsset(Constants.FighterSelect.WizardTextAsset, Constants.FighterSelect.NameX, Constants.FighterSelect.WizardY),

                ComputePlayerSelectAsset(0),
                ComputePlayerSelectAsset(1)
            };
        }

        private SelectScreenAsset ComputePlayerSelectAsset(int playerIndex)
        {
            var x = Constants.FighterSelect.NameX + playerIndex * Constants.FighterSelect.FighterWidth / 2;
            var y = Constants.FighterSelect.FighterYStart + (_playerFighterIndices[playerIndex] + 0.25) * Constants.FighterSelect.FighterHeight;

            System.Console.WriteLine("{0}: ({1}, {2})", playerIndex, x, y);

            return new SelectScreenAsset(PlayerSelectAssets[playerIndex], x, (int)y - playerIndex);
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePlayerOne();
            UpdatePlayerTwo();
        }

        private int NextIndex(int prev, int delta)
        {
            return Math.Min(Constants.FighterSelect.FighterCount - 1, Math.Max(0, prev + delta));
        }

        private void UpdatePlayerOne()
        {
            if (_inputHelper.HasNewKeyDown(Keys.W) ||
                    _inputHelper.PlayerHasNewButtonDown(0, Buttons.LeftThumbstickUp))
                _playerFighterIndices[0] = NextIndex(_playerFighterIndices[0], -1);
            else if (_inputHelper.HasNewKeyDown(Keys.S) ||
                    _inputHelper.PlayerHasNewButtonDown(0, Buttons.LeftThumbstickDown))
                _playerFighterIndices[0] = NextIndex(_playerFighterIndices[0], 1);
        }

        private void UpdatePlayerTwo()
        {
            if (_inputHelper.HasNewKeyDown(Keys.Up) ||
                    _inputHelper.PlayerHasNewButtonDown(1, Buttons.LeftThumbstickUp))
                _playerFighterIndices[1] = NextIndex(_playerFighterIndices[1], -1);
            else if (_inputHelper.HasNewKeyDown(Keys.Down) ||
                    _inputHelper.PlayerHasNewButtonDown(1, Buttons.LeftThumbstickDown))
                _playerFighterIndices[1] = NextIndex(_playerFighterIndices[1], 1);
        }
    }
}
