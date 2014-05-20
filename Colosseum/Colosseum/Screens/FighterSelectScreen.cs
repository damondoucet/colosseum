using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public override bool IsModal { get { return false; } }

        public FighterSelectScreen(ScreenManager screenManager)
            : base(screenManager)
        {
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
                new SelectScreenAsset(Constants.FighterSelect.WizardTextAsset, Constants.FighterSelect.NameX, Constants.FighterSelect.WizardY)
            };
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
