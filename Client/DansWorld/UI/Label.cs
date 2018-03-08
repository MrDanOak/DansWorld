using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI
{
    class Label : Control
    {
        public string Text = "";
        public Texture2D BackgroundImage;
        public SpriteFont Font;
        public Label()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (BackgroundImage != null)
            {

            }
            else if (BackColor != null)
            {
                spriteBatch.Draw(GameClient.DEFAULT_TEXTURE, Destination, BackColor);
                spriteBatch.DrawString(Font, Text, new Vector2(Location.X + Size.X / 2 - Font.MeasureString(Text).X / 2,
                    Location.Y + Size.Y / 2 - Font.MeasureString(Text).Y / 2), FrontColor);
            }
        }
    }
}
