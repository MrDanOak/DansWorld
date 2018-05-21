using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DansWorld.GameClient.UI.Scenes
{
    /// <summary>
    /// Base scene, defines basic behaviours
    /// </summary>
    public abstract class BaseScene
    {
        /// <summary>
        /// A list of controls that all inheriting classes will have access to
        /// </summary>
        public List<Control> Controls;
        /// <summary>
        /// Loading the content for the scene
        /// </summary>
        /// <param name="Content">Access to the monogame pipeline</param>
        public abstract void Initialise(ContentManager Content);
        /// <summary>
        /// Updating all the controls/sprites within the scene
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            foreach (Control control in Controls)
            {
                control.Update(gameTime);
            }
        }
        /// <summary>
        /// Drawing all the controls/sprites within the scene
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Control control in Controls)
            {
                control.Draw(gameTime, spriteBatch);
            }
        }
    }
}
