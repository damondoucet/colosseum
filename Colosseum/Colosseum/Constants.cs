using System;
namespace Colosseum
{
    static class Constants
    {
        public static class Assets
        {
            public static string BackgroundAsset = "background";
            public static string PlatformAsset = "platform_tile";

            public static string FighterBody = "fighter_body";
            public static string FighterHead = "fighter_head";
            public static string FighterWeapon = "fighter_weapon";
        }

        public static int Width = 1280;
        public static int Height = 720;

        private static float MaxJumpHeight = 256;
        private static float TimeToApex = 0.5f;

        // http://www.error454.com/2013/10/23/platformer-physics-101-and-the-3-fundamental-equations-of-platformers/
        public static float Gravity = 2 * MaxJumpHeight / TimeToApex / TimeToApex;

        public static float FighterJumpVelocity = (float)Math.Sqrt(2 * Gravity * MaxJumpHeight);
        public static float FighterMovementX = 10;

        public static float FighterWeaponDistance = 30.0f;

        public static float YPlatformCollisionAllowance = 10;  // pixels
    }
}
