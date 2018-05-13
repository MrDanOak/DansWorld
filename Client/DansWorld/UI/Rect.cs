using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI
{
    public class Rect : Control
    {
        public Color BorderColor = Color.Black;
        public int BorderThickness = 1;
        public Rect()
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (BorderColor != null && BorderThickness > 0)
            spriteBatch.Draw(GameClient.DEFAULT_TEXTURE, 
                new Rectangle(Location.X - BorderThickness, Location.Y - BorderThickness, Size.X + BorderThickness * 2, Size.Y + BorderThickness * 2), BorderColor);

            if (BackColor != null)
                spriteBatch.Draw(GameClient.DEFAULT_TEXTURE,Destination, BackColor);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
