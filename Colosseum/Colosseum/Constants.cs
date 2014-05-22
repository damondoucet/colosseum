using Microsoft.Xna.Framework;
using System;

namespace Colosseum
{
    static class Constants
    {
        public static class GameAssets
        {
            public static string[] WinAssets = new string[] { "p1_win", "p2_win" };

            public static string HitboxTestPoint = "hitbox_test_point";

            public static string Background = "background";
            public static string Platform = "platform_tile";

            public static string TestProjectile = "test_projectile";

            public static class Knight
            {
                public static string Head = "knight_head";
                public static string Body = "knight_body";
                public static string Weapon = "knight_weapon";

                public static string StunnedHead = "knight_head_stunned";

                public static string Thrust = "knight_thrust";
                public static string Shielding = "knight_shielding";
                public static string ShieldFlying = "knight_flying_shield";
                public static string ShieldSitting = "knight_sitting_shield";
            }

            public static class Ninja
            {
                public static string Head = "ninja_head";
                public static string Body = "ninja_body";
                public static string Weapon = "ninja_weapon";

                public static string StunnedHead = "ninja_head_stunned";

                public static string Thrust = "knight_thrust";

                public static string Bomb = "ninja_bomb";
                public static string BombExplosion = "ninja_explosion";
            }

            public static class Wizard
            {
                public static string Head = "wizard_head";
                public static string Body = "wizard_body";
                public static string Weapon = "wizard_weapon";

                public static string StunnedHead = "wizard_head_stunned";

                public static string Bomb = "wizard_bomb";
                public static string Cloud = "wizard_cloud";
                public static string CloudProjectile = "wizard_cloud_projectile";
                public static string ForcePulse = "wizard_force_pulse";
            }
        }

        public static class Fighters
        {
            public static float JumpSensitivity = 0.5f;

            public static int Width = 64;
            public static int Height = 64;

            private static float MaxJumpHeight = 256;
            private static float TimeToApex = 0.8f;

            // http://www.error454.com/2013/10/23/platformer-physics-101-and-the-3-fundamental-equations-of-platformers/
            public static float Gravity = 2 * MaxJumpHeight / TimeToApex / TimeToApex;

            public static float JumpVelocity = (float)Math.Sqrt(2 * Gravity * MaxJumpHeight);
            public static float XMovement = 7.5f;

            public static float WeaponDistance = 20.0f;  // pixels

            public static double ShieldCooldown = 10;  // seconds
            public static double BlinkPeriod = 1;
            public static double BlinkLength = 0.5;  // seconds
            public static Color BlinkTint = Color.Gray;

            public static Vector2 PlayerOneSpawn = new Vector2(350f, 300f);
            public static Vector2 PlayerTwoSpawn = new Vector2(950f, 300f);

            public static float SlowStrength = .35f;

            public static class Explosion
            {
                public static double Length = 0.75;  // seconds

                public static double Scale = 400;  // pixels/second

                public static float KnockbackForce = 1000f;
                public static float KnockbackTime = 0.2f;  // seconds; how long the knockback force is applied
            }

            public static class Knight
            {
                public static float DashVelocity = 1500f;  // pixels/second
                public static float DashTime = 0.1f;  // seconds
                public static float DashCooldown = 0.3f;  // seconds

                public static class Abilities
                {
                    public static class SwordSwing
                    {
                        public static double StartAngle = 4 * Math.PI / 6;
                        public static double EndAngle = 2 * Math.PI / 6;

                        private static double AttackTime = .25;  // seconds

                        public static double AngularVelocity = (EndAngle - StartAngle) / AttackTime;

                        public static double Cooldown = 0.5;  // seconds
                    }

                    public static class Thrust
                    {
                        public static double PhaseInTime = 0.5;  // seconds
                        public static double TimeToLive = .3;  // seconds

                        public static int Width = 80;
                        public static int Height = 7;

                        public static double Cooldown = 0.75;  // seconds
                    }

                    public static class Shield
                    {
                        public static double ArcLength = Math.PI / 4;
                        public static int DistanceFromBodyCenter = 75;  // pixels

                        public static float ShieldThrowVelocity = 1250;  // pixels/second

                        public static int FlyingAssetWidth = 20;
                        public static int FlyingAssetHeight = 7;

                        public static double StunLength = 2;  // seconds

                        public static int SittingAssetWidth = 30;
                        public static int SittingAssetHeight = 30;

                        public static int ShieldingAssetWidth = 100;
                        public static int ShieldingAssetHeight = 100;

                        // collision is estimated using a rectangle
                        // given an arc of a circle, we want the distance between the two corners to be the width of the rect
                        // that's finding the 3rd side of an isoc. triangle with equal sides of length R and a specific angle
                        // = law of cosines
                        public static double CollisionRectangleWidth = Math.Sqrt(2 * DistanceFromBodyCenter * DistanceFromBodyCenter * (1 - Math.Cos(ArcLength)));
                        public static double CollisionRectangleHeight = 10;  // pixels

                        // the rectangle is so that if the shield were facing straight up, the apex of the shield
                        // would be at rectCenter + (0, h/2)
                        // this means we can compute the apex (r * angleVector) and then find the center of the rect by subtracting
                        // h/2 * angleVector -> center = (r - h/2) * angleVector
                    }
                }
            }

            public static class Ninja
            {
                public static float DashVelocity = 1500f;  // pixels/second
                public static float DashTime = 0.1f;  // seconds
                public static float DashCooldown = 0.3f;  // seconds
                public static byte CloakOpacity = 50; // <256

                public static class Abilities
                {
                    public static class Clone
                    {
                        public static double InvisibilityTimeLength = 1;  // seconds

                        public static double CloneLifetime = 1.5;  // seconds

                        public static double SlowLength = 2;
                    }

                    public static class Thrust
                    {
                        public static double PhaseInTime = 0.5;  // seconds
                        public static double TimeToLive = 1;  // seconds

                        public static int Width = 80;
                        public static int Height = 7;

                        public static double Cooldown = 0.25;  // seconds
                    }

                    public static class Bomb
                    {
                        public static double DormantTime = 1.5;  // seconds
                    }

                    public static class Counter
                    {
                        public static double Duration = 0.25;  // seconds
                        public static double Cooldown = 1.5;  // seconds; excludes duration
                        public static int Radius = 150;  // pixels
                    }
                }
            }

            public static class Wizard
            {
                public static float DashVelocity = 1500f;  // pixels/second
                public static float DashTime = 0.1f;  // seconds
                public static float DashCooldown = 0.3f;  // seconds

                public static class Abilities
                {
                    public static class Triangle
                    {
                        public static float VelocityMagnitude = 1000.0f;  // pixels/second
                        public static float FireDistance = 20.0f;  // pixels
                        public static float Cooldown = 0.2f;  // seconds

                        public static int Width = 50;
                        public static int Height = 50;

                        public static float PhaseInTime = 0.5f;  // seconds
                        public static float TimeToLive = 2.0f;  // seconds
                    }

                    public static class Bomb
                    {
                        public static double PhaseInTime = 0.3;
                        public static double TimeToLive = 3;
                        public static float VelocityMagnitude = 400;

                        public static int Width = 50;
                        public static int Height = 50;

                        public static double Cooldown = 0.2;
                    }

                    public static class Cloud
                    {
                        public static int Width = 60;
                        public static int Height = 60;

                        public static int YOffset = -100;

                        public static double PhaseInTime = 1;
                        public static double TimeToLive = 5;

                        public static float VelocityMagnitude = 300;

                        public static double TimeBetweenProjectiles = 0.3;
                    }

                    public static class CloudProjectile
                    {
                        public static int Width = 20;
                        public static int Height = 20;

                        public static double PhaseInTime = 0.3;
                        public static double TimeToLive = 1000;  // should exit before this happens
                    }

                    public static class ForcePulse
                    {
                        public static double PhaseInTime = 0.2;
                        public static double TimeToLive = .1;

                        public static double Scale = 500;  // pixels/second
                        public static int Height = 70;

                        public static float KnockbackForce = 500f;
                        public static float KnockbackTime = 0.4f;  // seconds; how long the knockback force is applied

                        public static double Cooldown = 0.5;
                    }
                }
            }
        }

        public static class FighterSelect
        {
            public static string PlayerOneSelectAsset = "p1_select";
            public static string PlayerTwoSelectAsset = "p2_select";

            public static string LogoTextAsset = "logo_text";
            public static string KnightTextAsset = "select_knight";
            public static string NinjaTextAsset = "select_ninja";
            public static string WizardTextAsset = "select_wizard";
            public static string ReadyAsset = "ready";

            public static int[] PlayerReadyX = new[] { 30, 838 };
            public static int[] PlayerReadyY = new[] { 144, 144 };

            public static int LogoX = 0;
            public static int LogoY = 0;

            public static int FighterCount = 3;

            public static int NameX = 512;

            public static int FighterYStart = 144;
            public static int FighterWidth = 296;
            public static int FighterHeight = 144;

            public static int KnightIndex = 0;
            public static int NinjaIndex = 1;
            public static int WizardIndex = 2;

            public static int KnightY = FighterYStart + FighterHeight * KnightIndex;
            public static int NinjaY = FighterYStart + FighterHeight * NinjaIndex;
            public static int WizardY = FighterYStart + FighterHeight * WizardIndex;
        }

        public static double GameOverTimeBeforeTransition = 2;

        public static float ThumbstickSensitivity = 1E-10f;

        public static bool DisplayHitboxTestPoints = false;

        public static int Width = 1280;
        public static int Height = 720;
        public static int StageHeight = 704;

        public static float YSensitivity = 0.0002f;
        public static float YPlatformCollisionAllowance = 10;  // pixels

        public static Color GameOverTint = Color.Red;
        public static Color PausedTint = Color.Gray;
    }
}
