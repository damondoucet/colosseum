using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Colosseum
{
    class InputHelper
    {
        public enum Action
        {
            Left,
            Right,
            Jump,
            Dash,
            Projectile
        }

        private struct PlayerActionPair
        {
            public int PlayerIndex;
            public Action Action;

            public PlayerActionPair(int playerIndex, Action action)
            {
                PlayerIndex = playerIndex;
                Action = action;
            }
        }

        private static Dictionary<Keys, PlayerActionPair> KeyToPlayerActionPair = new Dictionary<Keys, PlayerActionPair>()
        {
            { Keys.A, new PlayerActionPair(0, Action.Left) },
            { Keys.D, new PlayerActionPair(0, Action.Right) },
            { Keys.W, new PlayerActionPair(0, Action.Jump) },
            { Keys.E, new PlayerActionPair(0, Action.Dash) },
            { Keys.R, new PlayerActionPair(0, Action.Projectile) },

            { Keys.Left, new PlayerActionPair(1, Action.Left) },
            { Keys.Right, new PlayerActionPair(1, Action.Right) },
            { Keys.Up, new PlayerActionPair(1, Action.Jump) },
            { Keys.L, new PlayerActionPair(1, Action.Dash) },
            { Keys.K, new PlayerActionPair(1, Action.Projectile) },
        };

        private static Dictionary<Buttons, Action> ButtonsToAction = new Dictionary<Buttons, Action>()
        {
            { Buttons.X, Action.Jump },
            { Buttons.A, Action.Dash },
            { Buttons.RightTrigger, Action.Projectile }
        };

        private readonly Fighter[] _fighters;

        private bool _hadPauseKeyDown;

        public InputHelper(Fighter[] fighters)
        {
            _fighters = fighters;

            _hadPauseKeyDown = false;
        }

        public bool ShouldTogglePause(GameTime gameTime)
        {
            var hasPause = Keyboard.GetState().IsKeyDown(Keys.Enter) ||
                    GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start) ||
                    GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Start);

            var ret = !_hadPauseKeyDown && hasPause;
            _hadPauseKeyDown = hasPause;
            return ret;
        }

        public void CheckInput()
        {
            var keyboard = Keyboard.GetState();
            var gamePadOne = GamePad.GetState(PlayerIndex.One);
            var gamePadTwo = GamePad.GetState(PlayerIndex.Two);

            if (gamePadOne.IsConnected || gamePadTwo.IsConnected)
                CheckGamePads(gamePadOne, gamePadTwo);
            else
                CheckKeyboardInput(keyboard);
        }

        private void CheckKeyboardInput(KeyboardState keyboard)
        {
            foreach (KeyValuePair<Keys, PlayerActionPair> kvp in KeyToPlayerActionPair)
            {
                var angle = 0;
                var vector = Util.VectorFromAngle(angle);

                if (keyboard.IsKeyDown(kvp.Key))
                    PostAction(kvp.Value, vector, vector);
            }
        }

        private void CheckGamePads(GamePadState one, GamePadState two)
        {
            if (!one.IsConnected || !two.IsConnected)
            {
                // TOOD: one of the controllers DC'd...
                Console.WriteLine("One of the controllers isn't working! Disregarding other controller's input anymore");
                return;
            }

            CheckGamePadInput(0, one);
            CheckGamePadInput(1, two);
        }

        private void PostAction(PlayerActionPair playerActionPair, Vector2 leftThumbstick, Vector2 rightThumbstick)
        {
            if (playerActionPair.PlayerIndex < 0)
                throw new Exception(
                    string.Format("Invalid player index: {0} for action {1} when trying to post input",
                        playerActionPair.PlayerIndex, playerActionPair.Action));

            if (playerActionPair.PlayerIndex > _fighters.Length)
            {
                Console.WriteLine("Player Index ({0}) for Action {1} does not exist. Eating action",
                    playerActionPair.PlayerIndex, playerActionPair.Action);
                return;
            }

            _fighters[playerActionPair.PlayerIndex].HandleAction(
                playerActionPair.Action, leftThumbstick, rightThumbstick);
        }

        private void CheckGamePadInput(int playerIndex, GamePadState gamePad)
        {
            foreach (KeyValuePair<Buttons, Action> kvp in ButtonsToAction)
                if (gamePad.IsButtonDown(kvp.Key))
                    PostAction(
                        new PlayerActionPair(playerIndex, kvp.Value), 
                        gamePad.ThumbSticks.Left,
                        gamePad.ThumbSticks.Right);

            _fighters[playerIndex].OnLeftThumbstick(gamePad.ThumbSticks.Left);
            _fighters[playerIndex].OnRightThumbstick(gamePad.ThumbSticks.Right);
        }
    }
}
