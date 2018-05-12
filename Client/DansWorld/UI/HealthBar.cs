using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansWorld.GameClient.UI.CustomEventArgs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI
{
    public class HealthBar : IControl
    {
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
            
        }

        public void Clicked(ClickedEventArgs e)
        {

        }

        public void SetHP(int hp, int max)
        {
            int percentage = (int)((float)100 * ((float)hp / (float)max));
            _currentHealth.Size = new Point(50 * percentage / 100, 10); 
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _maxHealth.Draw(gameTime, spriteBatch);
            _currentHealth.Draw(gameTime, spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _maxHealth.Update(gameTime);
            _currentHealth.Update(gameTime);
        }
    }
}
