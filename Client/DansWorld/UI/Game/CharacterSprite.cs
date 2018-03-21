using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DansWorld.GameClient.UI.Game
{
    public class CharacterSprite
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
        protected Label _namePlate;
        protected bool _mouseOver;

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

        public CharacterSprite()
        {
        }


        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
        }

        public virtual void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            _mouseOver = (mouseState.X > Destination.Left && mouseState.Y > Destination.Top &&
                          mouseState.X < Destination.Right && mouseState.Y < Destination.Bottom);
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
    }
}
