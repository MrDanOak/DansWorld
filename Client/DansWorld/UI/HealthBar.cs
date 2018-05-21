using DansWorld.GameClient.GameComponents;
using DansWorld.GameClient.UI.CustomEventArgs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI
{
    /// <summary>
    /// Health bars to display the current health of a player or enemy. Built using two rectangles and a scale system.
    /// </summary>
    public class HealthBar : Game.IDrawable
    {
        /// <summary>
        /// Visible timer is used to make the health bars disappear after an arbitrary amount of time
        /// </summary>
        int visibleTimer = 0;
        /// <summary>
        /// The current health value
        /// </summary>
        int currentValue;
        Rect _maxHealth = new Rect()
        {
            BorderThickness = 1,
            BorderColor = Color.Black,
            BackColor = Color.DarkRed,
            Size = new Point(52, 12),
            Location = new Point(0, 0)
        };

        Rect _currentHealth = new Rect()
        {
            BorderThickness = 0,
            BackColor = Color.Red, 
            Size = new Point(50, 10)
        };

        internal Point Size
        {
            get
            {
                return _maxHealth.Size;
            }
        }

        /// <summary>
        /// Location of the health bar
        /// </summary>
        public Point Location
        {
            get
            {
                return _maxHealth.Location;
            }
            set
            {
                _maxHealth.Location = value;
                _currentHealth.Location = new Point(value.X + 1, value.Y + 1);
            }
        }

        public HealthBar()
        {
            visibleTimer = 1000;
        }

        /// <summary>
        /// Sets the hp of the health bar
        /// </summary>
        /// <param name="hp">current hp value</param>
        /// <param name="max">maximum hp value</param>
        public void SetHP(int hp, int max)
        {
            //getting the scale of how large the current health bar should be relative to the maximum
            int percentage = (int)((float)100 * ((float)hp / (float)max));
            _currentHealth.Size = new Point(50 * percentage / 100, 10);
            if (hp != currentValue)
            {
                currentValue = hp;
                visibleTimer = 500;
            }
        }

        /// <summary>
        /// Draw method of the Health Bar
        /// </summary>
        /// <param name="gameTime">Game time in the XNA Framework</param>
        /// <param name="spriteBatch">Sprite batch in the XNA Framework</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (visibleTimer > 0)
            {
                _maxHealth.Draw(gameTime, spriteBatch);
                _currentHealth.Draw(gameTime, spriteBatch);
            }
        }

        /// <summary>
        /// Update method of the health bar
        /// </summary>
        /// <param name="gameTime">Game time in the XNA framework</param>
        /// <param name="camera">Camera reference in case the positional update relies on the camera position</param>
        public void Update(GameTime gameTime, Camera2D camera)
        {
            if (visibleTimer > 0)
            {
                //visible timer will disappear eventually
                visibleTimer -= 1;
            }
            _maxHealth.Update(gameTime);
            _currentHealth.Update(gameTime);
        }
    }
}
