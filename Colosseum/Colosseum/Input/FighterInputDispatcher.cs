using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosseum.Input
{
    class FighterInputDispatcher
    {
        public enum Action
        {
            Left,
            Right,
            Jump,
            Dash,
            Projectile,

            LeftShoulder,
            LeftTrigger,
            RightShoulder,
            RightTrigger
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

            { Keys.T, new PlayerActionPair(0, Action.LeftShoulder) },
            { Keys.G, new PlayerActionPair(0, Action.LeftTrigger) },
            { Keys.Y, new PlayerActionPair(0, Action.RightShoulder) },
            { Keys.H, new PlayerActionPair(0, Action.RightTrigger) },

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
            { Buttons.Y, Action.Projectile },
            { Buttons.LeftShoulder, Action.LeftShoulder },
            { Buttons.LeftTrigger, Action.LeftTrigger },
            { Buttons.RightShoulder, Action.RightShoulder },
            { Buttons.RightTrigger, Action.RightTrigger }
        };

        private readonly InputHelper _inputHelper;
        private readonly Fighter[] _fighters;

        public FighterInputDispatcher(InputHelper inputHelper, Fighter[] fighters)
        {
            _inputHelper = inputHelper;
            _fighters = fighters;
        }
        
        public void CheckInput()
        {
            var keyboard = Keyboard.GetState();
            var gamePadOne = GamePad.GetState(PlayerIndex.One);
            var gamePadTwo = GamePad.GetState(PlayerIndex.Two);

            if (gamePadOne.IsConnected || gamePadTwo.IsConnected)
                CheckGamePads(gamePadOne, gamePadTwo);
            else
                CheckKeyboardInput();
        }

        private void CheckKeyboardInput()
        {
            foreach (KeyValuePair<Keys, PlayerActionPair> kvp in KeyToPlayerActionPair)
            {
                var angle = kvp.Value.Action == Action.Left ? Math.PI : 0;
                var vector = Util.VectorFromAngle(angle);

                if (_inputHelper.HasKeyDown(kvp.Key))
                    PostAction(kvp.Value, true, vector, vector);
                else if (_inputHelper.ReleasedKey(kvp.Key))
                    PostAction(kvp.Value, false, vector, vector);
            }
        }

        private void CheckGamePads(GamePadState one, GamePadState two)
        {
            if (!one.IsConnected || !two.IsConnected)
            {
                // TOOD: one of the controllers DC'd...
                return;
            }

            CheckGamePadInput(0, one);
            CheckGamePadInput(1, two);
        }

        private void PostAction(PlayerActionPair playerActionPair, bool pressed, Vector2 leftThumbstick, Vector2 rightThumbstick)
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
                playerActionPair.Action, pressed, leftThumbstick, rightThumbstick);
        }

        private void CheckGamePadInput(int playerIndex, GamePadState gamePad)
        {
            foreach (KeyValuePair<Buttons, Action> kvp in ButtonsToAction)
            {
                if (_inputHelper.PlayerHasButtonDown(playerIndex, kvp.Key))
                    PostAction(
                        new PlayerActionPair(playerIndex, kvp.Value),
                        true,
                        gamePad.ThumbSticks.Left,
                        gamePad.ThumbSticks.Right);
                else if (_inputHelper.PlayerReleasedButton(playerIndex, kvp.Key))
                    PostAction(
                        new PlayerActionPair(playerIndex, kvp.Value),
                        false,
                        gamePad.ThumbSticks.Left,
                        gamePad.ThumbSticks.Right);
            }
            
            _fighters[playerIndex].OnLeftThumbstick(gamePad.ThumbSticks.Left);
            _fighters[playerIndex].OnRightThumbstick(gamePad.ThumbSticks.Right);
        }
    }
}
