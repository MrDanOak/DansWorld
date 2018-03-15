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
    public class PlayerCharacterSprite : CharacterSprite
    {
        public Rectangle SourceRectangle;
        public PlayerCharacter PlayerCharacter;
        public bool InGame = false;

        private Rectangle Dest
        {
            get
            {
                return new Rectangle(Location, Size);
            }
        }

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
        public PlayerCharacterSprite()
        {
            _namePlate = new Label()
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
                if (InGame)
                {
                    if (_mouseOver)
                    {
                        _namePlate.Draw(gameTime, spriteBatch);
                    }
                    float depth = 1 - ((float)Location.Y / GameClient.HEIGHT);


                    spriteBatch.Draw(Texture,
                        new Rectangle(PlayerCharacter.X, PlayerCharacter.Y, Width, Height),
                        GetRectangleForFrameID(_animationID + (PlayerCharacter.Gender == Gender.FEMALE ? 3 : 0) + _facingModifier),
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        depth);
                }
                else
                {
                    spriteBatch.Draw(Texture,
                        Destination,
                        GetRectangleForFrameID(_animationID + (PlayerCharacter.Gender == Gender.FEMALE ? 3 : 0) + _facingModifier), 
                        Color.White);
                }
            } 
        }

        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (PlayerCharacter == null) return;


            if (InGame)
            {
                Location = new Point(PlayerCharacter.X, PlayerCharacter.Y);

                if (PlayerCharacter.IsWalking)
                {
                    _animationTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (_animationTimer > 200)
                    {
                        _animationID += 1;
                        if (_animationID == 3)
                            _animationID = 0;
                        _animationTimer = 0;
                    }
                }
                else
                {
                    _animationID = 1;
                }

                if (_mouseOver)
                {
                    if (_namePlate.Text == null || _namePlate.Text == "" && PlayerCharacter != null)
                    {
                        _namePlate.Text = PlayerCharacter.Name;
                    }
                    Vector2 dimCharName = GameClient.DEFAULT_FONT.MeasureString(PlayerCharacter.Name);
                    _namePlate.Size = new Point((int)dimCharName.X, (int)dimCharName.Y);
                    _namePlate.Location = new Point(PlayerCharacter.X + (Width / 2) - ((int)dimCharName.X / 2), PlayerCharacter.Y - (int)dimCharName.Y);
                    _namePlate.Update(gameTime);
                }
            }
            else
            {
                _animationID = 1;
            }
        }
    }
}
