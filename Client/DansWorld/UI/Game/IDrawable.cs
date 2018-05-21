using DansWorld.GameClient.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI.Game
{
    /// <summary>
    /// Enforces update and draw functionality
    /// </summary>
    public interface IDrawable
    {
        void Update(GameTime gameTime, Camera2D camera);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
