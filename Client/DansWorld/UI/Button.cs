using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI
{
    /// <summary>
    /// Custom button class for Dan's World
    /// </summary>
    class Button : Control
    {
        /// <summary>
        /// Visible text of the button
        /// </summary>
        public string Text = "";
        public Texture2D BackgroundImage;
        /// <summary>
        /// font for the button to use
        /// </summary>
        public SpriteFont Font;
        /// <summary>
        /// Base constructor for the button
        /// </summary>
        public Button()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            if (!IsVisible) return;
            if (BackgroundImage != null)
            {

            }
            else if (BackColor != null)
            {
                //drawing a rectangle to back the button
                spriteBatch.Draw(GameClient.DEFAULT_TEXTURE, Destination, BackColor);
                //drawing the text within the button
                spriteBatch.DrawString(Font, Text, new Vector2(Location.X + Size.X / 2 - Font.MeasureString(Text).X / 2, 
                    Location.Y + Size.Y / 2 - Font.MeasureString(Text).Y / 2), FrontColor);
            }
        }
    }
}
