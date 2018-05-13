using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DansWorld.GameClient.UI.Scenes
{
    public abstract class BaseScene
    {
        public List<Control> Controls;
        public abstract void Initialise(ContentManager Content);
        public virtual void Update(GameTime gameTime)
        {
            foreach (Control control in Controls)
            {
                control.Update(gameTime);
            }
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Control control in Controls)
            {
                control.Draw(gameTime, spriteBatch);
            }
        }
    }
}
