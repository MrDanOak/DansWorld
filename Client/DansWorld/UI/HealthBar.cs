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
    class HealthBar : IControl
    {
        Rect _maxHealth = new Rect()
        {
            BorderThickness = 1,
            BorderColor = Color.Black,
            BackColor = Color.Red
        };
        Rect _currentHealth = new Rect()
        {
            BorderThickness = 0
        };
        public void Clicked(ClickedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
