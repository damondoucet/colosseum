using Colosseum.GameObjects;
using Colosseum.GameObjects.Fighters;
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
        private static FighterType[] IndexToFighterType = new[] { FighterType.Knight, FighterType.Ninja, FighterType.Wizard };
        
        private int[] _playerFighterIndices;
        private bool[] _isReady;

        public override bool IsModal { get { return false; } }

        private readonly InputHelper _inputHelper;

        public FighterSelectScreen(ScreenManager screenManager, InputHelper inputHelper)
            : base(screenManager)
        {
            _playerFighterIndices = new[] { 0, 0 };
            _isReady = new[] { false, false };
            _inputHelper = inputHelper;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isTopMost)
        {
            foreach (var asset in ComputeAssets())
                asset.Draw(spriteBatch);
        }

        private List<SelectScreenAsset> ComputeAssets()
        {
            var ret = new List<SelectScreenAsset>()
            {
                new SelectScreenAsset(Constants.FighterSelect.LogoTextAsset, Constants.FighterSelect.LogoX, Constants.FighterSelect.LogoY),
                new SelectScreenAsset(Constants.FighterSelect.KnightTextAsset, Constants.FighterSelect.NameX, Constants.FighterSelect.KnightY),
                new SelectScreenAsset(Constants.FighterSelect.NinjaTextAsset, Constants.FighterSelect.NameX, Constants.FighterSelect.NinjaY),
                new SelectScreenAsset(Constants.FighterSelect.WizardTextAsset, Constants.FighterSelect.NameX, Constants.FighterSelect.WizardY),

                ComputePlayerSelectAsset(0),
                ComputePlayerSelectAsset(1)
            };

            if (_isReady[0])
                ret.Add(ComputeReadyAsset(0));
            if (_isReady[1])
                ret.Add(ComputeReadyAsset(1));

            return ret;
        }

        private SelectScreenAsset ComputePlayerSelectAsset(int playerIndex)
        {
            var x = Constants.FighterSelect.NameX + playerIndex * Constants.FighterSelect.FighterWidth / 2;
            var y = Constants.FighterSelect.FighterYStart + (_playerFighterIndices[playerIndex] + 0.25) * Constants.FighterSelect.FighterHeight;

            return new SelectScreenAsset(PlayerSelectAssets[playerIndex], x, (int)y - playerIndex);
        }

        private SelectScreenAsset ComputeReadyAsset(int playerIndex)
        {
            return new SelectScreenAsset(
                Constants.FighterSelect.ReadyAsset,
                Constants.FighterSelect.PlayerReadyX[playerIndex],
                Constants.FighterSelect.PlayerReadyY[playerIndex]);
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePlayer(0, Keys.W, Keys.S, Keys.E);
            UpdatePlayer(1, Keys.Up, Keys.Down, Keys.OemQuotes);

            if (ShouldStart())
                StartGame();
        }

        private bool ShouldStart()
        {
            return (_inputHelper.HasKeyDown(Keys.Enter) ||
                    _inputHelper.PlayerHasButtonDown(0, Buttons.Start) ||
                    _inputHelper.PlayerHasButtonDown(1, Buttons.Start)) &&
                    _isReady[0] &&
                    _isReady[1];
        }

        private void StartGame()
        {
            var stage = new Stage();
            var fighterFactory = new FighterFactory(stage);

            Fighter[] fighters = new[]
            {
                fighterFactory.CreateFighter(IndexToFighterType[_playerFighterIndices[0]], 1),
                fighterFactory.CreateFighter(IndexToFighterType[_playerFighterIndices[1]], 2)
            };

            ScreenManager.PushScreen(new FightScreen(ScreenManager, _inputHelper, stage, fighters));
        }

        private int NextIndex(int prev, int delta)
        {
            return Math.Min(Constants.FighterSelect.FighterCount - 1, Math.Max(0, prev + delta));
        }

        private void UpdatePlayer(int playerIndex, Keys up, Keys down, Keys select)
        {
            // this kinda sucks :/
            if (_inputHelper.HasNewKeyDown(select) ||
                    _inputHelper.PlayerHasNewButtonDown(playerIndex, Buttons.A))
                _isReady[playerIndex] = !_isReady[playerIndex];

            else if (!_isReady[playerIndex])
            {
                if (_inputHelper.HasNewKeyDown(up) ||
                        _inputHelper.PlayerHasNewButtonDown(playerIndex, Buttons.LeftThumbstickUp))
                    _playerFighterIndices[playerIndex] = NextIndex(_playerFighterIndices[playerIndex], -1);

                else if (_inputHelper.HasNewKeyDown(down) ||
                        _inputHelper.PlayerHasNewButtonDown(playerIndex, Buttons.LeftThumbstickDown))
                    _playerFighterIndices[playerIndex] = NextIndex(_playerFighterIndices[playerIndex], 1);
            }
        }
    }
}
