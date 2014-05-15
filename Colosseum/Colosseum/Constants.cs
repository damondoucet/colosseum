using Microsoft.Xna.Framework;
using System;
namespace Colosseum
{
    static class Constants
    {
        public static class Assets
        {
            public static string GameOver = "game_over";

            public static string HitboxTestPoint = "hitbox_test_point";

            public static string Background = "background";
            public static string Platform = "platform_tile";

            public static string FighterBody = "fighter_body";
            public static string FighterHead = "fighter_head";
            public static string FighterWeapon = "fighter_weapon";

            public static string TestProjectile = "test_projectile";
        }

        public static class Fighters
        {
            public static class Knight
            {
                public static float DashVelocity = 1500f;  // pixels/second
                public static float DashTime = 0.1f;  // seconds
            }
        }

        public static class Projectiles
        {
            public static class Test
            {
                public static float VelocityMagnitude = 1000.0f;  // pixels/second
                public static float FireDistance = 20.0f;  // pixels
                public static float Cooldown = 0.2f;  // seconds

                public static int Width = 50;
                public static int Height = 50;

                public static float PhaseInTime = 0.5f;  // seconds
                public static float TimeToLive = 2.0f;  // seconds
            }
        }

        public static float ThumbstickSensitivity = 1E-10f;

        public static bool DisplayHitboxTestPoints = false;

        public static int Width = 1280;
        public static int Height = 720;

        private static float MaxJumpHeight = 256;
        private static float TimeToApex = 0.5f;

        // http://www.error454.com/2013/10/23/platformer-physics-101-and-the-3-fundamental-equations-of-platformers/
        public static float Gravity = 2 * MaxJumpHeight / TimeToApex / TimeToApex;

        public static float FighterJumpVelocity = (float)Math.Sqrt(2 * Gravity * MaxJumpHeight);
        public static float FighterMovementX = 10;
        
        public static double FighterDashCooldown = 0.3;  // seconds

        public static float FighterWeaponDistance = 20.0f;  // pixels

        public static float YPlatformCollisionAllowance = 10;  // pixels

        public static double ShieldCooldown = 10;  // seconds
        public static double BlinkPeriod = 1;
        public static double BlinkLength = 0.5;  // seconds
        public static Color BlinkTint = Color.Gray;

        public static Color GameOverTint = Color.Red;
        public static Color PausedTint = Color.Gray;

        public static Vector2 PlayerOneSpawn = new Vector2(350f, 300f);
        public static Vector2 PlayerTwoSpawn = new Vector2(950f, 300f);
    }
}
