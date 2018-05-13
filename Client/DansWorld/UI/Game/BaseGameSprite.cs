using DansWorld.GameClient.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DansWorld.GameClient.UI.Game
{
    public class BaseGameSprite : IDrawable
    {
        public Texture2D Texture;
        public Point Location;
        public Point Size;
        public bool IsVisible;
        public int frameID;
        public int Width;
        public int Height;
        protected int _animationTimer = 0;
        protected int _animationID = 1;
        protected bool _mouseOver;

        public Vector2 Position
        {
            get
            {
                return new Vector2(Location.X + Width / 2, Location.Y + Height / 2);
            }
        }

        protected int _framesWide
        {
            get
            {
                if (Texture != null) return Texture.Width / Width;
                else return -1;
            }
        }

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

        public Rectangle GetRectangleForFrameID(int id)
        {
            Rectangle rect = new Rectangle();
            int frameX, frameY;
            int framesWide;
            if (Texture != null)
            {
                framesWide = Texture.Width / Width;
                frameX = id % framesWide;
                frameY = id / framesWide;
                rect = new Rectangle(frameX * Width, frameY * Height, Width, Height);
            }
            return rect;
        }

        public void Update(GameTime gameTime, Camera2D camera)
        {
            MouseState mouseState = Mouse.GetState();
            float mousex = (camera == null ? mouseState.X : camera.Position.X - camera.ScreenCenter.X + mouseState.X);
            float mousey = (camera == null ? mouseState.Y : camera.Position.Y - camera.ScreenCenter.Y + mouseState.Y);
            _mouseOver = (mousex > Destination.Left && mousey > Destination.Top &&
                          mousex < Destination.Right && mousey < Destination.Bottom);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
        }
    }
}
