using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Colosseum.GameObjects.Fighters
{
    enum FighterType
    { 
        Knight,
        Ninja
    }

    class FighterFactory
    {
        private static Dictionary<int, Vector2> PlayerIndexToStartingPosition = new Dictionary<int, Vector2>()
        {
            { 1, Constants.PlayerOneSpawn },
            { 2, Constants.PlayerTwoSpawn }
        };

        private static Dictionary<int, float> PlayerIndexToStartingAngle = new Dictionary<int, float>()
        {
            { 1, 0f },
            { 2, -MathHelper.Pi }
        };

        private readonly Stage _stage;

        public FighterFactory(Stage stage)
        {
            _stage = stage;
        }

        /// <param name="playerIndex">1 or 2</param>
        public Fighter CreateFighter(FighterType type, int playerIndex)
        {
            switch (type)
            { 
                case FighterType.Knight:
                    return CreateKnight(playerIndex);
                case FighterType.Ninja:
                    return CreateNinja(playerIndex);
                default:
                    throw new Exception("Invalid FighterType: " + type.ToString());
            }
        }

        private Fighter CreateKnight(int playerIndex)
        {
            return new Knight(
                _stage, 
                PlayerIndexToStartingPosition[playerIndex], 
                PlayerIndexToStartingAngle[playerIndex]);
        }

        private Fighter CreateNinja(int playerIndex)
        {
            return new Ninja(
                _stage, 
                PlayerIndexToStartingPosition[playerIndex], 
                PlayerIndexToStartingAngle[playerIndex]);
        }
    }
}
