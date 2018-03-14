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
        public PlayerCharacter PlayerCharacter;
        public bool InGame = false;
        private Label _characterLabel;
        private int animationTimer = 0;
        private int animationID = 1;

        private int _facingModifier
        {
            get
            {
                int i = 0;
                switch (PlayerCharacter.Facing)
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

            if (PlayerCharacter != null)
            {
                if (_mouseOver)
                {
                    _characterLabel.Draw(gameTime, spriteBatch);
                }
                float depth = 1 - ((float)Location.Y / GameClient.HEIGHT);

                spriteBatch.Draw(BaseTexture, 
                    (InGame ? new Rectangle(PlayerCharacter.X, PlayerCharacter.Y, SpriteWidth, SpriteHeight) : Destination), 
                    GetRectangleForFrameID(animationID + (PlayerCharacter.Gender == Gender.FEMALE ? 3 : 0) + _facingModifier), 
                    Color.White, 
                    0.0f, 
                    Vector2.Zero, 
                    SpriteEffects.None, 
                    depth);
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
            base.Update(gameTime);
            if (PlayerCharacter == null) return;

            //walkTimerTick += gameTime.ElapsedGameTime.Milliseconds;

            //if (previousCharacterLoc != null && previousCharacterLoc.X == Character.X && previousCharacterLoc.Y == Character.Y 
            //    && Character.IsWalking && walkTimerTick > 300)
            //{
            //    Character.IsWalking = false;
            //    walkTimerTick = 0;
            //}

            Location = new Point(PlayerCharacter.X, PlayerCharacter.Y);

            if (PlayerCharacter.IsWalking)
            {
                animationTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (animationTimer > 200)
                {
                    animationID += 1;
                    if (animationID == 3)
                        animationID = 0;
                    animationTimer = 0;
                }
            }
            else
            {
                animationID = 1;
            }

            if (_mouseOver)
            {
                if (_characterLabel.Text == null || _characterLabel.Text == "" && PlayerCharacter != null)
                {
                    _characterLabel.Text = PlayerCharacter.Name;
                }
                Vector2 dimCharName = GameClient.DEFAULT_FONT.MeasureString(PlayerCharacter.Name);
                _characterLabel.Size = new Point((int)dimCharName.X, (int)dimCharName.Y);
                _characterLabel.Location = new Point(PlayerCharacter.X + (SpriteWidth / 2) - ((int)dimCharName.X / 2), PlayerCharacter.Y - (int)dimCharName.Y);
                _characterLabel.Update(gameTime);
            }
            //previousCharacterLoc = new Point(Character.X, Character.Y);
        }
    }
}
