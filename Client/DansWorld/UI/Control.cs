using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using DansWorld.GameClient.UI.CustomEventArgs;

namespace DansWorld.GameClient.UI
{
    /// <summary>
    /// Base control class, abstract for behaviours to be overriden with this base class still eligable to be called.
    /// </summary>
    public abstract class Control : IControl
    {
        /// <summary>
        /// Name of the control
        /// </summary>
        public string Name = "";
        /// <summary>
        /// Location of the control
        /// </summary>
        public Point Location = new Point(0, 0);
        /// <summary>
        /// Size of the control
        /// </summary>
        public Point Size = new Point(0, 0);
        /// <summary>
        /// Background color of the control
        /// </summary>
        public Color BackColor;
        /// <summary>
        /// Foreground colour of the control
        /// </summary>
        public Color FrontColor;
        /// <summary>
        /// Does the control have focus? / does it want to subscribe to inputs?
        /// </summary>
        public bool HasFocus;
        /// <summary>
        /// Event handler for clicking on the control
        /// </summary>
        public event EventHandler<ClickedEventArgs> OnClick;
        /// <summary>
        /// Is the control visible?
        /// </summary>
        public bool IsVisible = true;
        /// <summary>
        /// Mouse down inside the control?
        /// </summary>
        protected bool _mouseDownInside = false;
        /// <summary>
        /// Mouse released inside the control
        /// </summary>
        protected bool _mouseUpInside = false;
        /// <summary>
        /// Mouse is hovering over the control
        /// </summary>
        protected bool _mouseOver = false;
        /// <summary>
        /// Rectangle of where the control is positioned based on the location and size of the control
        /// </summary>
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
            //Evaluating if the mouse is hovering over the control
            _mouseOver = (mouseState.X > Destination.Left && mouseState.Y > Destination.Top &&
                          mouseState.X < Destination.Right && mouseState.Y < Destination.Bottom);
            
            _mouseUpInside = (mouseState.LeftButton == ButtonState.Released && _mouseOver);
            //mouse was released inside the control after the mouse was put down in the control
            if (_mouseDownInside && _mouseUpInside)
            {
                Clicked(new ClickedEventArgs(mouseState.Position));
            }
            _mouseDownInside = (mouseState.LeftButton == ButtonState.Pressed && _mouseOver);
            
        }

        public virtual void Clicked(ClickedEventArgs e)
        {
            OnClick?.Invoke(this, e);
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
        }

    }
}
