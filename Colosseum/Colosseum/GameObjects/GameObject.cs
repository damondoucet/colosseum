using Colosseum.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Colosseum.GameObjects
{
    abstract class GameObject
    {
        public Stage Stage { get; set; }

        public  Vector2 TopLeftPosition;  // this isn't a standard property because we want to be able to do Position.Y += ...

        public GameObject(Stage stage, Vector2 topLeftPosition)
        {
            Stage = stage;
            TopLeftPosition = topLeftPosition;
        }

        protected abstract List<Asset> ComputeAssets();

        public virtual void Draw(SpriteBatch batch, GameTime gameTime)
        {
            ComputeAssets().ForEach(asset => asset.Draw(batch));
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
