using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansWorld.Common.GameEntities;
using DansWorld.Common.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI.Game
{
    public class CharacterSprite : Control
    {
        public Texture2D BaseTexture;
        public Rectangle SourceRectangle;
        public int SpriteWidth;
        public int SpriteHeight;
        public int frameID;
        public Character Character;
        public bool InGame = false;
        private Label _characterLabel;
        private int _facingModifier
        {
            get
            {
                int i = 0;
                switch (Character.Facing)
                {
                    case Direction.DOWN: i = 0; break;
                    case Direction.LEFT: i = 12; break;
                    case Direction.RIGHT: i = 24; break;
                    case Direction.UP: i = 36; break;
                }
                return i;
            }
        }
        public CharacterSprite()
        {
            _characterLabel = new Label()
            {
                FrontColor = Color.Black,
                Font = GameClient.DEFAULT_FONT
            };
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (Character != null)
            {
                if (_mouseOver)
                {
                    if (_characterLabel.Text == null || _characterLabel.Text == "") _characterLabel.Text = Character.Name;

                    _characterLabel.Draw(gameTime, spriteBatch);
                }
                if (Character.IsIdle)
                {
                    switch (Character.Gender)
                    {
                        case Gender.MALE: spriteBatch.Draw(BaseTexture, (InGame ? new Rectangle(Character.X, Character.Y, SpriteWidth, SpriteHeight) : Destination), GetRectangleForFrameID(1 + _facingModifier), Color.White); break;
                        case Gender.FEMALE: spriteBatch.Draw(BaseTexture, (InGame ? new Rectangle(Character.X, Character.Y, SpriteWidth, SpriteHeight) : Destination), GetRectangleForFrameID(4 + _facingModifier), Color.White); break;
                    }
                }
            }
        }

        public Rectangle GetRectangleForFrameID(int id)
        {
            Rectangle rect = new Rectangle();
            int frameX, frameY;
            int framesWide;
            if (BaseTexture != null)
            {
                framesWide = BaseTexture.Width / SpriteWidth;
                frameX = id % framesWide;
                frameY = id / framesWide;
                rect = new Rectangle(frameX * SpriteWidth, frameY * SpriteHeight, SpriteWidth, SpriteHeight);
            }
            return rect;
        }

        public new void Update(GameTime gameTime)
        {
            if (Character != null) 
                Location = new Point(Character.X, Character.Y);

            base.Update(gameTime);

            if (_mouseOver)
            {
                if (_characterLabel.Text == null || _characterLabel.Text == "" && Character != null)
                {
                    _characterLabel.Text = Character.Name;
                }
                Vector2 dimCharName = GameClient.DEFAULT_FONT.MeasureString(Character.Name);
                _characterLabel.Size = new Point((int)dimCharName.X, (int)dimCharName.Y);
                _characterLabel.Location = new Point(Character.X + (SpriteWidth / 2) - ((int)dimCharName.X / 2), Character.Y - (int)dimCharName.Y);
                _characterLabel.Update(gameTime);
            }
        }
    }
}
