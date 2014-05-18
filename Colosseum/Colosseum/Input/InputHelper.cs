using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Colosseum.Input
{
    class InputHelper
    {
        private KeyboardState _prevKeyboard;
        private GamePadState[] _prevGamePads;

        private KeyboardState _curKeyboard;
        private GamePadState[] _curGamePads;

        public InputHelper()
        {
            UpdateStates();  // prev = null, cur populated
            UpdateStates();  // prev populated, cur populated
        }

        public void UpdateStates()
        {
            _prevKeyboard = _curKeyboard;
            _prevGamePads = _curGamePads;

            _curKeyboard = Keyboard.GetState();
            _curGamePads = new[] { GamePad.GetState(PlayerIndex.One), GamePad.GetState(PlayerIndex.Two) };
        }

        public bool PauseToggled()
        {
            var hasPauseKey = HasPause(
                _curKeyboard,
                _curGamePads[0],
                _curGamePads[1]);
            
            var hadPauseKey = HasPause(
                _prevKeyboard,
                _prevGamePads[0],
                _prevGamePads[1]);

            return !hadPauseKey && hasPauseKey;
        }

        private bool HasPause(KeyboardState keyboard, GamePadState gamePadOne, GamePadState gamePadTwo)
        {
            return keyboard.IsKeyDown(Keys.Enter) ||
                    gamePadOne.IsButtonDown(Buttons.Start) ||
                    gamePadTwo.IsButtonDown(Buttons.Start);
        }

        public bool HasKeyDown(Keys key)
        {
            return _curKeyboard.IsKeyDown(key);
        }

        public bool ReleasedKey(Keys key)
        {
            return _prevKeyboard.IsKeyDown(key) && !_curKeyboard.IsKeyDown(key);
        }

        public bool PlayerHasButtonDown(int playerIndex, Buttons button)
        {
            return _curGamePads[playerIndex].IsButtonDown(button);
        }

        public bool PlayerReleasedButton(int playerIndex, Buttons button)
        {
            return !PlayerHasButtonDown(playerIndex, button) &&
                _prevGamePads[playerIndex].IsButtonDown(button);
        }
    }
}
