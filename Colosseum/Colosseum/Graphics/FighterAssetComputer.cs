using Colosseum.GameObjects.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Colosseum.Graphics
{
    class FighterAssetComputer
    {
        public List<Asset> ComputeAssets(Fighter fighter, Vector2 topLeftPosition, Color tint)
        {
            var bodySize = TextureDictionary.FindTextureSize(fighter.BodyAsset);
            var headSize = TextureDictionary.FindTextureSize(fighter.HeadAsset);
            var weaponSize = TextureDictionary.FindTextureSize(fighter.WeaponAsset);

            var headPosition = topLeftPosition + new Vector2((bodySize.X - headSize.X) / 2, -headSize.Y);
            var headAngle = fighter.WeaponAngle + (Util.IsAngleLeft(fighter.WeaponAngle) ? (float)Math.PI : 0);

            var weaponPosition = topLeftPosition + fighter.ComputeWeaponOffset() - weaponSize / 2.0f;

            var spriteEffects = fighter.IsFacingLeft() ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            return new List<Asset>()
            {
                new Asset(fighter.Stage, fighter.BodyAsset, topLeftPosition, 0.0f, tint, spriteEffects),
                new Asset(fighter.Stage, fighter.HeadAsset, headPosition, headAngle, tint, spriteEffects),

                // we don't flip the weapon since the rotation will handle that for us
                new Asset(fighter.Stage, fighter.WeaponAsset, weaponPosition, fighter.WeaponAngle, tint, SpriteEffects.None)
            };
        }
    }
}
