using DansWorld.GameClient.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI.Game
{
    public class CharacterSprite : BaseGameSprite, ICameraFocusbale, IDrawable
    {
        public HealthBar HealthBar;
        protected Label _namePlate;

        public CharacterSprite()
        {
            HealthBar = new HealthBar();
        }


        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public virtual void Update(GameTime gameTime, Camera2D camera)
        {
            base.Update(gameTime, camera);
        }
    }
}
