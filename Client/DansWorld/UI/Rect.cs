using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            spriteBatch.Draw(GameClient.DEFAULT_TEXTURE, 
                new Rectangle(Location.X - BorderThickness, Location.Y - BorderThickness, Size.X + BorderThickness * 2, Size.Y + BorderThickness * 2), BorderColor);
            spriteBatch.Draw(GameClient.DEFAULT_TEXTURE,Destination, BackColor);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
