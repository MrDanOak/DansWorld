using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansWorld.GameClient.UI.CustomEventArgs;

namespace DansWorld.GameClient.UI
{
    public abstract class Control
    {
        public string Name = "";
        public Point Location = new Point(0, 0);
        public Point Size = new Point(0, 0);
        public Color BackColor;
        public Color FrontColor;
        public bool HasFocus;
        public event EventHandler<ClickedEventArgs> OnClick;

        protected bool _mouseDownInside = false;
        protected bool _mouseUpInside = false;
        protected bool _mouseOver = false;

        public Rectangle Destination
        {
            get
            {
                return new Rectangle(Location, Size);
            }
            set
            {
                Location = value.Location;
                Size = value.Size;
            }
        }
        public virtual void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            _mouseOver = (mouseState.X > Destination.Left && mouseState.Y > Destination.Top &&
                          mouseState.X < Destination.Right && mouseState.Y < Destination.Bottom);
            
            _mouseUpInside = (mouseState.LeftButton == ButtonState.Released && _mouseOver);
            if (_mouseDownInside && _mouseUpInside)
            {
                Clicked(new ClickedEventArgs(mouseState.Position));
            }
            _mouseDownInside = (mouseState.LeftButton == ButtonState.Pressed && _mouseOver);
            
        }

        protected virtual void Clicked(ClickedEventArgs e)
        {
            OnClick?.Invoke(this, e);
        }
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    }
}
